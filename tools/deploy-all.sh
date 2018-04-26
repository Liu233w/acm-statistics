set -e
cd ..
find . -maxdepth 2 -name 'package-lock.json' -delete
git pull
cd crawler
npm i
cd ../crawler-api-backend
npm i --unsafe-perm
cd ../frontend
npm i --unsafe-perm
npm run build
systemctl restart crawler-backend-api.acm-statistics.service frontend.acm-statistics.service
