# Ejercicio 2 - Sistema de Simulación de Tráfico Mejorado

## Descripción
Se implementa un sistema de simulación de tráfico en tiempo real con comunicación cliente-servidor. El sistema permite la gestión de vehículos en una carretera virtual, con actualizaciones en tiempo real y manejo de múltiples clientes.

## Mejoras Implementadas sobre el Ejercicio 1

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

