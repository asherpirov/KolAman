import json
from confluent_kafka import Consumer

from validator import validate_alert
from redis_service import is_duplicate
from geo_service import get_region_with_geopandas
from rabbit_producer import send_to_rabbit
from elastic_service import log_alert

config = {
    "bootstrap.servers": "localhost:9092",
    "group.id": "alerts-group",
    "auto.offset.reset": "earliest"
}

consumer = Consumer(config)
consumer.subscribe(["alerts"])

try:
    while True:
        msg = consumer.poll(1.0)
        if msg is None:
            continue

        if msg.error():
            log_alert(None, command=f"kafka_error: {msg.error()}")
            continue

        raw = msg.value()
        print("RAW MESSAGE:", raw)

        try:
            decoded = raw.decode("utf-8")
        except Exception:
            decoded = str(raw)

        try:
            alert = json.loads(decoded)
        except Exception as ex:
            log_alert(None, command=f"invalid_json: {ex}", raw_message=decoded)
            continue

        if not validate_alert(alert):
            log_alert(alert, command="validation_failed")
            continue

        if is_duplicate(alert["alert_id"]):
            log_alert(alert, command=f"duplicate_alert: {alert['alert_id']}")
            continue

        command = get_region_with_geopandas("regions.geojson", alert["lon"], alert["lat"])
        send_to_rabbit(command, alert)
        log_alert(alert, command=command)

finally:
    consumer.close()