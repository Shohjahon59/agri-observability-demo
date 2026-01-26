# 🌾 Agri Observability Demo

Modern qishloq xo'jaligi uchun mikroservis arxitekturasi va to'liq observability stack bilan qurilgan demo loyha.

## 📋 Loyha haqida

Bu loyha .NET 8 da yozilgan FoodAPI mikroservisi va uning atrofida qurilgan to'liq DevOps ekotizimini namoyish etadi. Loyha zamonaviy cloud-native yondashuvlar, GitOps va observability best practice larini qo'llaydi.

## 🏗️ Arxitektura

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   GitHub Repo   │───▶│  GitHub Actions │───▶│   Docker Hub    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                                ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│     ArgoCD      │◀───│  Helm Charts    │    │   Kubernetes    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                                        │
                                                        ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Prometheus    │◀───│     FoodAPI     │───▶│      Loki       │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🚀 Texnologiyalar

### Backend
- **Framework:** .NET 8 (ASP.NET Core)
- **Metrics:** Prometheus.NET
- **Logging:** Microsoft.Extensions.Logging
- **Health Checks:** Built-in ASP.NET Core

### DevOps & Infrastructure
- **CI/CD:** GitHub Actions
- **Container Registry:** Docker Hub
- **Orchestration:** Kubernetes
- **Package Manager:** Helm
- **GitOps:** ArgoCD

### Observability Stack
- **Metrics:** Prometheus + Grafana
- **Logs:** Loki + Grafana
- **Monitoring:** Kube-Prometheus-Stack

## 📁 Loyha strukturasi

```
agri-observability-demo/
├── src/FoodApi/                    # .NET 8 mikroservis
│   └── FoodApi/
│       ├── Controllers/            # API kontrollerlar
│       ├── Observability/          # Metrics va monitoring
│       ├── Program.cs              # Asosiy dastur
│       └── Dockerfile              # Container image
├── charts/foodapi/                 # Helm chart
│   ├── templates/                  # Kubernetes manifests
│   ├── Chart.yaml                  # Chart metadata
│   └── values.yaml                 # Konfiguratsiya
├── deploy/
│   ├── argocd/                     # ArgoCD application
│   └── monitoring/                 # Observability configs
└── .github/workflows/              # CI/CD pipeline
```

## 🛠️ O'rnatish va ishga tushirish

### Talablar
- Docker
- Kubernetes cluster
- Helm 3.x
- ArgoCD (ixtiyoriy)

### 1. Repository ni clone qiling
```bash
git clone https://github.com/ogash3103/agri-observability-demo.git
cd agri-observability-demo
```

### 2. Docker image ni build qiling
```bash
docker build -t foodapi:latest -f src/FoodApi/FoodApi/Dockerfile .
```

### 3. Helm chart orqali deploy qiling
```bash
# Namespace yarating
kubectl create namespace agri

# Helm chart ni o'rnating
helm install foodapi ./charts/foodapi -n agri
```

### 4. ArgoCD orqali GitOps (ixtiyoriy)
```bash
# ArgoCD application ni yarating
kubectl apply -f deploy/argocd/foodapi-app.yaml
```

## 📊 API Endpoints

### Health Check
```http
GET /health
```
**Response:**
```json
{
  "ok": true
}
```

### Order yaratish
```http
POST /orders
```
**Response:**
```json
{
  "status": "created"
}
```

### Metrics
```http
GET /metrics
```
Prometheus format da metrics qaytaradi.

## 📈 Monitoring va Metrics

### Custom Metrics
- `foodapi_orders_created_total` - Yaratilgan orderlar soni
- `foodapi_http_requests_total` - HTTP so'rovlar soni
- `foodapi_http_request_duration_seconds` - So'rov davomiyligi

### Default .NET Metrics
- GC metrics
- Thread pool metrics
- HTTP metrics
- va boshqalar...

## 🔄 CI/CD Pipeline

GitHub Actions orqali avtomatik CI/CD pipeline:

1. **Trigger:** `src/FoodApi/**` yoki `charts/foodapi/**` o'zgarishida
2. **Build:** Docker image yaratish
3. **Push:** Docker Hub ga yuklash
4. **Update:** Helm values.yaml da image tag ni yangilash
5. **Deploy:** ArgoCD avtomatik sync qiladi

### Pipeline xususiyatlari
- Avtomatik image tag generation (git commit SHA)
- Conflict resolution mechanism
- Retry logic bilan ishonchli push
- `[skip ci]` tag bilan infinite loop oldini olish

  <img width="3022" height="1774" alt="image" src="https://github.com/user-attachments/assets/a5f32eee-04cc-4565-b891-f858b0c42124" />


## 🎯 Observability

### Prometheus Metrics
```yaml
# ServiceMonitor orqali avtomatik discovery
apiVersion: monitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: foodapi
spec:
  selector:
    matchLabels:
      app: foodapi
  endpoints:
  - port: http
    path: /metrics
```

<img width="3024" height="1964" alt="image" src="https://github.com/user-attachments/assets/b32af751-b100-4940-a5fc-bbb7eb29f200" />

<img width="3024" height="1202" alt="image" src="https://github.com/user-attachments/assets/4a12c69c-38a6-4f5f-8ceb-18b125f16629" />



### Grafana Dashboard
Quyidagi metrikalar uchun dashboard yarating:
- Request rate (RPS)
- Response time (latency)
- Error rate
- Order creation rate
  
<img width="3024" height="1964" alt="image" src="https://github.com/user-attachments/assets/485c7d98-b333-451a-a4ae-b0f22e81707b" />

## 🔧 Konfiguratsiya

### Environment Variables
```yaml
# values.yaml da
env:
  - name: ASPNETCORE_ENVIRONMENT
    value: "Production"
  - name: ASPNETCORE_URLS
    value: "http://+:8080"
```

### Resource Limits
```yaml
resources:
  requests:
    memory: "64Mi"
    cpu: "50m"
  limits:
    memory: "128Mi"
    cpu: "100m"
```

## 🚦 Health Checks

Kubernetes readiness va liveness probe lar:
```yaml
livenessProbe:
  httpGet:
    path: /health
    port: 8080
  initialDelaySeconds: 30
  periodSeconds: 10

readinessProbe:
  httpGet:
    path: /health
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 5
```

## 🔐 Security

### Image Security
- Multi-stage Docker build
- Non-root user
- Minimal base image (aspnet:8.0)

### Kubernetes Security
- Resource limits
- Security context
- Network policies (ixtiyoriy)

## 📚 Qo'shimcha ma'lumotlar

### Foydali buyruqlar
```bash
# Pod loglarini ko'rish
kubectl logs -f deployment/foodapi -n agri

# Metrics tekshirish
kubectl port-forward svc/foodapi 8080:80 -n agri
curl http://localhost:8080/metrics

# ArgoCD sync
argocd app sync foodapi
```

### Troubleshooting
1. **Pod ishlamayapti:** `kubectl describe pod <pod-name> -n agri`
2. **Image pull xatosi:** Docker Hub credentials tekshiring
3. **Metrics ko'rinmayapti:** ServiceMonitor konfiguratsiyasini tekshiring

## 🤝 Hissa qo'shish

1. Repository ni fork qiling
2. Feature branch yarating (`git checkout -b feature/amazing-feature`)
3. O'zgarishlarni commit qiling (`git commit -m 'Add amazing feature'`)
4. Branch ni push qiling (`git push origin feature/amazing-feature`)
5. Pull Request yarating

## 📄 Litsenziya

Bu loyha MIT litsenziyasi ostida tarqatiladi. Batafsil ma'lumot uchun `LICENSE` faylini ko'ring.

## 👨‍💻 Muallif

**Ogabek Gafurov**
- GitHub: [@ogash3103](https://github.com/ogash3103)
- Docker Hub: [ogabek0331](https://hub.docker.com/u/ogabek0331)

---

⭐ Agar loyha foydali bo'lsa, star bosishni unutmang!
