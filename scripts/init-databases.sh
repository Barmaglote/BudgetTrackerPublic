#!/bin/bash

mongosh -u mongoadmin -p mongopassword <<EOF
use loginapi;
use budgettracker;
use paymentapi;
sh.enableSharding("loginapi");
sh.shardCollection("loginapi.users", { "_id": 1 });
sh.enableSharding("budgettracker")
sh.shardCollection("budgettracker.income", { "_id": 1 })
sh.shardCollection("budgettracker.expenses", { "_id": 1 })
sh.enableSharding("paymentapi");
sh.shardCollection("paymentapi.payments", { "_id": 1 });
EOF