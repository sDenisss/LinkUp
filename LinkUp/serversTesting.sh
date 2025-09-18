# In git bash I need to write ->  ./serversTesting.sh

# Проверка статики через HTTPS
curl -k https://localhost/css/home.css
curl -k https://localhost/js/login.js

for i in {1..6}; do
  echo "Request $i:"
  curl -k -I https://localhost | grep "X-Server-ID"
  echo "---"
done

# # Проверка балансировки через HTTPS
# for i in {1..6}; do
#   echo "Request $i:"
#   curl -k -s -I https://localhost | grep "X-Server-ID"
#   sleep 0.3
# done

# # Health check
# curl -k https://localhost/nginx-health
