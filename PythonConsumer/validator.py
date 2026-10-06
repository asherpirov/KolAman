MIN_LAT = -90
MAX_LAT = 90

MIN_LON = -180
MAX_LON = 180

VALIDATE_PRIORITIES = ["CRITICAL", "HIGH","MEDIUM", "LOW"]
VALIDATE_CLASSIFICATIONS = ["UNCLASSIFIED", "RESTRICTED", "SECRET", "TOP_SECRET"]
VALIDATE_SOURCES = ["aman", "mossad", "pikud-haoref", "shabak"]
VALIDATE_STSTUS = "WAITING"


def validate_alert(alert: dict) -> bool:

    required_fields = [
        "alert_id", "source",
        "title", "content",
        "priority", "classification",
        "lat", "lon", "timestamp", "status"
    ]

    text_fields = [
        "alert_id", "source",
        "title", "content","priority",
        "classification","status"
    ]

    for field in required_fields:
        if field not in alert:
            return False

    for field in text_fields:    
        if not isinstance(alert[field], str):
            return False

    if not isinstance(alert["lat"], (int, float)):
        return False

    if not isinstance(alert["lon"], (int, float)):
        return False

    if not MIN_LAT <= alert["lat"] <= MAX_LAT:
        return False

    if not MIN_LON <= alert["lon"] <= MAX_LON:
        return False

    if not alert["priority"] in VALIDATE_PRIORITIES:
        return False

    if not alert["classification"] in VALIDATE_CLASSIFICATIONS:
        return False

    if not alert["source"] in VALIDATE_SOURCES:
        return False

    if alert["status"] != VALIDATE_STSTUS:
        return False
    

    return True
