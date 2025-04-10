# Ejercicio 3 - Control de Paso por el Túnel

## Explicacion

No he conseguido llegar a tiempo para realizar el ejercicio 3 y tendre que dejar la practica en el 2, aun asi voy a contestar a las preguntas teoricas que pide la practica en el ejercicio 3 para asi intentar puntuar algo.

## Pregunta Teórica 1: Explica las ventajas e inconvenientes de programar el control de paso por el túnel, en el cliente o en el servidor

### Control en el Servidor

**Ventajas:**
1. **Centralización del Estado**: El servidor mantiene una única fuente de verdad sobre el estado del túnel.
2. **Consistencia**: Elimina posibles condiciones de carrera entre clientes.
4. **Sincronización**: Facilita la coordinación entre múltiples clientes.

**Inconvenientes:**
1. **Carga de Procesamiento**: El servidor debe manejar toda la lógica de control.
2. **Latencia**: Cada decisión requiere comunicación cliente-servidor.
4. **Complejidad**: El servidor debe manejar múltiples estados y transiciones.

### Control en el Cliente

**Ventajas:**
2. **Carga Distribuida**: El procesamiento se distribuye entre los clientes.
4. **Escalabilidad**: Menor carga en el servidor.

**Inconvenientes:**
1. **Inconsistencia**: Riesgo de estados inconsistentes entre clientes.
2. **Seguridad**: Dificultad para garantizar el cumplimiento de reglas.
3. **Complejidad**: Los clientes deben implementar lógica compleja.
4. **Sincronización**: Mayor dificultad para coordinar acciones.


## Pregunta Teórica 2: Explica cómo gestionarías las colas de espera en el servidor. ¿Qué estructura de datos usarías para priorizar vehículos según su dirección? Justifica tu respuesta.

### Estructura de datos posible

La solución que yo usaria sería implementar dos colas prioritarias independientes, una para cada dirección (norte y sur). Cada cola utilizaría un sistema de prioridades que considera varios factores para determinar el orden de paso de los vehículos.

### Justificación Técnica

1. **Separación por Dirección**
   - Facilita el control del flujo de tráfico en cada sentido

2. **Sistema de Prioridades**
   - El tiempo de espera sería un factor determinante: los vehículos que esperan más tiempo tendrían mayor prioridad
   - La dirección actual del tráfico podría influir en la prioridad
   - La cantidad de vehículos esperando en cada dirección podría afectar las decisiones

3. **Consideraciones de Sincronización**
   - Se deberia implementar mecanismos de bloqueo para garantizar operaciones atómicas
   - Se deben prevenir condiciones de carrera en el acceso a las colas
