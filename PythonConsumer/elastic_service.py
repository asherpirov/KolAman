from elasticsearch import Elasticsearch
from datetime import datetime, timezone

es = Elasticsearch("http://localhost:9200")



def log_alert(alert, command, raw_message=None):
    
    alert_id = alert.get("alert_id") if isinstance(alert, dict) else None
    source = alert.get("source") if isinstance(alert, dict) else None
    priority = alert.get("priority") if isinstance(alert, dict) else None
    classification = alert.get("classification") if isinstance(alert, dict) else None
    log = {
        "alert_id": alert_id,
        "source": source,
        "priority" : priority,
        "classification" : classification,
        "command": command,
        "timestamp": datetime.now(timezone.utc).isoformat()
    }
    if raw_message is not None:
        log["raw"] = raw_message

    print("LOG:", log)

    es.index(
        index="alerts-logs",
        document=log)
    