import pymongo
from urllib.parse import quote_plus
from Types.Exchange.KLine import KLine
import traceback


class IwbrDb:
    def __init__(self, config):
        # Config.MongoDb fields
        self.host = config.MongoDb.host
        self.port = config.MongoDb.port
        self.username = config.MongoDb.username
        self.password = config.MongoDb.password
        self.database = config.MongoDb.database

        # Connect to MongoDB
        self.connect();
        self.close();


    def connect(self):
        try:
            connection_string = f"mongodb://{quote_plus(self.username)}:{quote_plus(self.password)}@{self.host}:{self.port}/{self.database}?maxPoolSize=1500&minPoolSize=30"
            self.client = pymongo.MongoClient(connection_string)
            self.db = self.client[self.database]
            print("Connected to MongoDB successfully!")
        except pymongo.errors.ConnectionFailure as e:
            print("Failed to connect to MongoDB:", e)

    def close(self):
        try:
            if self.client:
                self.client.close()
                print("Connection to MongoDB closed.")
        except Exception as e:
            print("Error closing MongoDB connection:", e)


    def get_last_klines(self, symbol, interval, n=1000):
        try:
            collection_name = "klines"

            # Connect to the MongoDB database
            self.connect()

            # Get the "klines" collection
            klines_collection = self.db[collection_name]

            # Query to get the last n documents sorted by lastUpdateTime in descending order
            query = {
                "symbol": symbol,
                "interval": interval
            }
            projection = {"_id": 0}
            sort_key = "lastUpdateTime"  # Use the correct sort key here
            cursor = klines_collection.find(query, projection).sort(sort_key, pymongo.DESCENDING).limit(n)

            # Convert the BSON documents to a list of KLine objects
            last_klines = [KLine(**kline_doc) for kline_doc in cursor]

            if not last_klines:
                print(f"No klines found for symbol: {symbol}, interval: {interval} from {self.database}.{collection_name}")

            # Sort the last_klines list based on lastUpdateTime in ascending order (ASC)
            last_klines_sorted = sorted(last_klines, key=lambda x: x.lastUpdateTime)

            return last_klines_sorted

        except Exception as e:
            print("Error retrieving last klines:", e)
            traceback.print_exc()
            return []

        finally:
            # Close the connection to the database
            self.close()