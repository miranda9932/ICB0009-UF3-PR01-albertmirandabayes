# ICB0009-UF3-PR01 - Ejercicio 1: Conexión Cliente-Servidor

## 📝 Descripción
Implementación de un sistema cliente-servidor para simular vehículos circulando por una carretera, con comunicación mediante sockets TCP y manejo de múltiples clientes concurrentes.

## 🏗️ Estructura del Proyecto
Ejercicio1/
├── Servidor/
│ ├── Program.cs # Lógica principal del servidor
│ └── Servidor.csproj
├── Cliente/
│ ├── Program.cs # Lógica del cliente vehicular
│ └── Cliente.csproj
└── Shared/
└── NetworkStreamClass.cs 

## Diagrama Conceptual

    A[Cliente] -->|Conexión TCP| B[Servidor]
    B --> C[Base de Datos de Estados]
    B --> D[Módulo de Broadcast]
    D --> E[Cliente 1]
    D --> F[Cliente 2]
    C --> G[Panel de Monitorización]


### Modelo Cliente-Servidor
El sistema implementa una arquitectura centralizada donde:
- **Servidor**: Coordina todas las conexiones vehiculares, mantiene el estado global y distribuye actualizaciones
- **Clientes**: Representan vehículos independientes con capacidad de:
  - Autenticación inicial
  - Reporte periódico de posición
  - Recepción de actualizaciones en tiempo real

### Componentes Principales
1. **Módulo de Conexión**
   - Protocolo de handshake en 3 pasos
   - Asignación automática de IDs únicos
   - Gestión de direcciones (Norte/Sur)

2. **Módulo de Comunicación**
   - Canal full-duplex sobre TCP/IP
   - Formato estructurado de mensajes
   - Mecanismo de difusión (broadcast) eficiente

3. **Módulo de Simulación**
   - Temporización de movimientos
   - Visualización de estados
   - Reglas de tráfico programables


### 🤝 Handshake (3 pasos)
1. **Cliente → Servidor**: Envía `"INICIO"`
2. **Servidor → Cliente**: Responde `"ID:[número]:[dirección]"`
3. **Cliente → Servidor**: Confirma con `"ACK:[ID]"`

### 📨 Mensajes durante operación
- `POS:[valor]` - Posición actual del vehículo
- `UPDATE:[ID]:[valor]` - Broadcast de actualizaciones

### Captura de pantalla salida por consola final: 

- Servidor:
<img width="984" alt="Captura de pantalla 2025-04-04 a las 22 25 48" src="https://github.com/user-attachments/assets/aee45b3b-2832-452b-9e15-fa0040f70080" />

- Cliente 1:
<img width="984" alt="Captura de pantalla 2025-04-04 a las 22 26 36" src="https://github.com/user-attachments/assets/d92c6f9e-01d5-49b8-bc11-1d2f87af62be" />

- Cliente 2:
<img width="984" alt="Captura de pantalla 2025-04-04 a las 22 27 07" src="https://github.com/user-attachments/assets/7bfcf8d7-700d-4e1c-bbfd-97dca8270500" />

- Cliente 3:
<img width="984" alt="Captura de pantalla 2025-04-04 a las 22 27 31" src="https://github.com/user-attachments/assets/ca176594-ee36-4dd8-895b-909dd13ced7d" />









