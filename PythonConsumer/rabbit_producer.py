import json
import pika


connection = pika.BlockingConnection(
    pika.ConnectionParameters(host="localhost")
)

channel = connection.channel()

queues = ["NORTH", "CENTER", "SOUTH", "OVERSEAS"]

for queue in queues:
    channel.queue_declare(
        queue=queue,
        durable=True
    )


def send_to_rabbit(command, alert):
    message = json.dumps(alert)

    channel.basic_publish(
        exchange="",
        routing_key=command,
        body=message
    )