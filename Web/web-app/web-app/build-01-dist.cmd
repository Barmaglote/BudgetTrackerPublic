call npm version minor 
call node replace.build.js ./src/environments/environment.prod.ts
call ng build --configuration production
pause
