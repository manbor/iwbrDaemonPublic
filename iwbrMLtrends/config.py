import json

class MongoDb:
    def __init__(self, host, port, username, password, database):
        self.host = host
        self.port = port
        self.username = username
        self.password = password
        self.database = database

class Config:
    def __init__(self, mongo_data):
        self.MongoDb = MongoDb(
            host=mongo_data["MongoDb"]["Host"],
            port=mongo_data["MongoDb"]["Port"],
            username=mongo_data["MongoDb"]["Username"],
            password=mongo_data["MongoDb"]["Password"],
            database=mongo_data["MongoDb"]["Database"]
        )
