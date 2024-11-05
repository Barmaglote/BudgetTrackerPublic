#!/bin/bash

mongosh -u mongoadmin -p mongopassword <<EOF
use budgettracker;
db.income.createIndex({ OwnerUserId: 1, Date: 1 })
db.income.createIndex({ OwnerUserId: 1 })
db.expenses.createIndex({ OwnerUserId: 1, Date: 1 })
db.expenses.createIndex({ OwnerUserId: 1 })
db.usersettings.createIndex({ OwnerUserId: 1 })
db.planning.createIndex({ OwnerUserId: 1 })
db.credits.createIndex({ OwnerUserId: 1 })
EOF