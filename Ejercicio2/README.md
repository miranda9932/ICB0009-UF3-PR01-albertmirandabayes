# Ejercicio 2 - Sistema de Simulación de Tráfico Mejorado

## Descripción
Se implementa un sistema de simulación de tráfico en tiempo real con comunicación cliente-servidor. El sistema permite la gestión de vehículos en una carretera virtual, con actualizaciones en tiempo real y manejo de múltiples clientes.

## Implementaciones ejercicio 2: 

### 1. Manejo de Conexiones Mejorado
- Implementación de `ConcurrentDictionary` para gestionar múltiples clientes de forma segura
- Sistema de IDs únicos para cada cliente
- Manejo robusto de desconexiones y errores de red

### 2. Comunicación en Tiempo Real
- Protocolo de comunicación mejorado con prefijos de mensaje
- Sistema de broadcast para actualizaciones de estado
- Handshake inicial para establecer conexión y dirección

### 3. Sincronización y Seguridad
- Uso de `lock` para operaciones críticas
- Manejo seguro de actualizaciones de estado
- Prevención de condiciones de carrera

### 4. Gestión de Estado
- Sistema de copias seguras para broadcast
- Actualización eficiente de posiciones de vehículos
- Detección de finalización de viajes

### Bibliotecas Principales
- `System.Net.Sockets`: Para la comunicación de red
- `System.Text.Json`: Para la serialización/deserialización de datos
- `System.Collections.Concurrent`: Para colecciones thread-safe

### Estructuras de Datos
- `ConcurrentDictionary`: Para almacenamiento seguro de clientes
- `List<Vehiculo>`: Para gestión de vehículos
- `NetworkStream`: Para comunicación de red

### Patrones de Diseño
- Modelo Cliente-Servidor
- Patrón Observer para actualizaciones de estado
- Manejo de concurrencia con locks


## Funcionalidades Principales

### Servidor
- Gestión de múltiples clientes simultáneamente
- Broadcast de actualizaciones de estado
- Manejo seguro de conexiones y desconexiones
- Sincronización de datos entre clientes

### Cliente
- Conexión segura al servidor
- Actualización de posición en tiempo real
- Recepción de estado global
- Manejo de errores de red

## Protocolo de Comunicación
1. **Handshake Inicial**
   - Cliente envía: "INICIO"
   - Servidor responde: "ID:DIRECCIÓN"
   - Cliente confirma: "ACK:ID"

2. **Actualizaciones de Estado**
   - Formato: "CARR:{...}"
   - Contiene información de todos los vehículos

3. **Mensajes de Control**
   - "ESPERA": Indica espera activa
   - "FIN": Finalización de conexión

## Capturas de pantalla de cliente y servidores por etapas: 

1. **Etapa 2:**

- Servidor:

<img width="780" alt="Salida por pantalla servidor " src="https://github.com/user-attachments/assets/307842b1-be42-4e45-9f7c-8545ce17185b" />
- Cliente 1:

<img width="974" alt="Salida por consola cliente 1" src="https://github.com/user-attachments/assets/fba5fa82-0082-4a31-b373-9869d64df4d9" />
- Cliente 2:

<img width="974" alt="Salida por consola cliente 2" src="https://github.com/user-attachments/assets/0113acdc-2b7e-4567-befe-e25d01ee6b45" />
- Cliente 3:

<img width="974" alt="Salida por pantalla cliente 3" src="https://github.com/user-attachments/assets/6e6f90ea-b56a-4c36-b4b4-0fc2677fb621" />


2. **Etapa 3:**

- Servidor (parcialmente):

<img width="1166" alt="Salida por consola servidor 1:2" src="https://github.com/user-attachments/assets/77b78d71-55c8-4b2a-92ef-2a3c3c15002c" />

- Cliente 1:

<img width="1166" alt="Salida consola cliente 1" src="https://github.com/user-attachments/assets/5c31656b-1964-4e20-adc8-bf8a689b540d" />

- Cliente 2:

<img width="1166" alt="Salida consola cliente 2" src="https://github.com/user-attachments/assets/256d17a2-817d-4bfa-a90a-77223227817f" />

3. **Etapa 4 y 5:**

- Servidor:

<img width="1507" alt="Captura de pantalla 2025-04-08 a las 11 43 44" src="https://github.com/user-attachments/assets/74947f53-bc03-4ae7-b4b1-b83bb459286f" />

- Cliente 1:

<img width="1507" alt="Captura de pantalla 2025-04-08 a las 11 43 19" src="https://github.com/user-attachments/assets/77065b59-ad77-4725-98dc-8561ead0b75b" />

- Cliente 2:

<img width="1507" alt="Captura de pantalla 2025-04-08 a las 11 43 13" src="https://github.com/user-attachments/assets/488c8aec-5616-4f97-a975-2418a2e8c37b" />

- Cliente 3:

<img width="1507" alt="Captura de pantalla 2025-04-08 a las 11 43 08" src="https://github.com/user-attachments/assets/10a1855f-74ec-467a-ab9b-b52c5a83514a" />

















