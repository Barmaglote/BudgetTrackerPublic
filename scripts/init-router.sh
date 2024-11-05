#!/bin/bash

mongosh <<EOF
sh.addShard("shard-01-data-replica-set/mongo-shard-01-arbiter:27017");
sh.addShard("shard-01-data-replica-set/mongo-shard-01-data-01:27017");
sh.addShard("shard-01-data-replica-set/mongo-shard-01-data-02:27017");
sh.addShard("shard-02-data-replica-set/mongo-shard-02-arbiter:27017");
sh.addShard("shard-02-data-replica-set/mongo-shard-02-data-01:27017");
sh.addShard("shard-02-data-replica-set/mongo-shard-02-data-02:27017");
EOF