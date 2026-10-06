import redis

r = redis.Redis(
    host="localhost",
    port=6379,
    decode_responses=True
)

def is_duplicate(alert_id):
    if r.exists(alert_id):
        return True

    r.set(alert_id, "seen", ex=300)

    return False