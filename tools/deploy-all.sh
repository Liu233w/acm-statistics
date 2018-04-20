find . -maxdepth 2 -name 'package-lock.json' -delete
git pull
cd ../crawler
npm i
cd ../crawler-api-backend
npm i
cd ../frontend
npm i
npm run build
systemctl restart crawler-backend-api.acm-statistics.service frontend.acm-statistics.service
