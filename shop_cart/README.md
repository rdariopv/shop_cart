# 🛒 Shop Cart — Blazor Web App

Frontend de carrito de compras construido con **Blazor Web App (.NET 8)**. Consume la API REST `store-api` para obtener productos y registrar órdenes. Persiste el carrito en `localStorage` del navegador.

---

## 📐 Arquitectura

```
┌─────────────────────────────────┐         ┌──────────────────────────────┐
│         shop_cart               │         │          store-api            │
│      (Blazor Web App)           │         │       (ASP.NET Core API)      │
│                                 │         │                              │
│  Pages/                         │         │  GET  /api/products          │
│   ├─ Home.razor                 │  HTTP   │  GET  /api/products/{id}     │
│   ├─ Products.razor  ───────────┼────────►│  GET  /api/products/image/{id}│
│   ├─ Cart.razor                 │         │  POST /api/orders            │
│   └─ Checkout.razor ────────────┼────────►│                              │
│                                 │◄────────┤  Devuelve OrderResponse      │
│  Services/                      │         └──────────────────────────────┘
│   ├─ CartService                │
│   ├─ CartState                  │
│   ├─ CartStorage ───────────────┼──► localStorage (navegador)
│   ├─ ProductService             │    key: "shopping_cart"
│   └─ OrderService               │
└─────────────────────────────────┘
```

---

## 🗂️ Estructura de Archivos

```
shop_cart/
├── Components/
│   ├── App.razor                  # Raíz — rendermode y scripts Blazor
│   ├── Layout/
│   │   ├── MainLayout.razor       # Layout con navbar y sidebar
│   │   └── NavBar.razor           # Barra de búsqueda + contador carrito
│   └── Pages/
│       ├── Home.razor             # Página principal
│       ├── Products.razor         # Lista de productos con búsqueda ?q=
│       ├── ProductCard.razor      # Tarjeta individual de producto
│       ├── Cart.razor             # Detalle del carrito
│       └── Checkout.razor         # Formulario de pago
│
├── Models/
│   ├── Product.cs                 # Modelo de producto
│   ├── CartItem.cs                # Item del carrito
│   ├── OrderRequest.cs            # DTO enviado a store-api
│   ├── OrderResponse.cs           # Respuesta de store-api
│   ├── CustomerInfo.cs            # Datos del cliente
│   ├── CardInfo.cs                # Datos de tarjeta
│   └── OrderItemDto.cs            # Item individual de la orden
│
├── Services/
│   ├── CartState.cs               # Estado en memoria del carrito
│   ├── CartStorage.cs             # Persistencia en localStorage
│   ├── CartService.cs             # API pública del carrito + QR
│   ├── ProductService.cs          # Consulta productos a store-api
│   └── OrderService.cs            # Envía órdenes a store-api
│
├── wwwroot/
│   ├── app.css
│   └── bootstrap/
│
└── Program.cs                     # DI, SignalR, HttpClients
```

---

## ⚙️ Capas del Carrito

El carrito usa una arquitectura de **3 capas** para separar responsabilidades:

| Clase | Responsabilidad |
|---|---|
| `CartState` | Estado en memoria. Contiene `List<CartItem>` y dispara `OnChange` |
| `CartStorage` | Lee y escribe en `localStorage` via `Blazored.LocalStorage` |
| `CartService` | Orquesta `CartState` + `CartStorage`. Es el único que los componentes inyectan |

```
Componente
    │  @inject CartService
    ▼
CartService
    ├──► CartState    (mutación en memoria + evento OnChange)
    └──► CartStorage  (persistencia en localStorage)
                │
                ▼
         localStorage
         key: "shopping_cart"
```

### Reglas de uso en componentes

```csharp
// ✅ Suscribir con método nombrado (para poder desuscribir en Dispose)
CartService.OnChange += OnCartChanged;

// ✅ InvokeAsync es obligatorio — OnChange viene de hilo externo
private async void OnCartChanged()
    => await InvokeAsync(StateHasChanged);

// ✅ InitializeAsync solo en OnAfterRenderAsync(firstRender)
// LocalStorage requiere JS interop — no disponible antes
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await CartService.InitializeAsync();
        StateHasChanged();
    }
}

// ✅ Siempre desuscribir en Dispose para evitar memory leaks
public void Dispose() => CartService.OnChange -= OnCartChanged;
```

---

## 🔄 Flujo Completo Post-Pago

```
┌─────────────────────────────────────────────────────────────┐
│  1. PROCESAMIENTO DEL PAGO           Estado: implementado ✅ │
│                                                             │
│  Usuario llena Checkout.razor                               │
│      │                                                      │
│      ▼                                                      │
│  Validación en cliente (nombre, email, items no vacíos)     │
│      │                                                      │
│      ▼                                                      │
│  OrderService.SubmitOrderAsync(OrderRequest)                │
│      │  POST /api/orders                                    │
│      ▼                                                      │
│  store-api — OrdersController                               │
│      ├─ Valida datos                                        │
│      ├─ Recalcula total en servidor                         │
│      ├─ Guarda en BD                                        │
│      └─ Devuelve OrderResponse { Success, OrderId, Total }  │
│      │                                                      │
│      ▼                                                      │
│  CartService.ClearAsync() → limpia estado + localStorage    │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│  2. CONFIRMACIÓN                     Estado: implementado ✅ │
│                                                             │
│  Checkout.razor muestra pantalla de éxito con:             │
│   • Número de orden (OrderId)                               │
│   • Total pagado                                            │
│   • Método de pago utilizado                                │
│   • Botón "Volver al inicio"                                │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│  3. COMPROBANTE / RECIBO             Estado: pendiente 🔲   │
│                                                             │
│  Página /orden/{orderId} con:                               │
│   • Resumen completo de la compra                           │
│   • Datos del cliente                                       │
│   • Detalle de productos (nombre, cantidad, precio)         │
│   • Total y método de pago                                  │
│   • QR de verificación de la orden                          │
│   • Botón "Imprimir" → window.print() con CSS @media print  │
│   • Botón "Descargar PDF" → generado en store-api           │
│                                                             │
│  Archivos a crear:                                          │
│   • Pages/OrderDetail.razor      (@page "/orden/{orderId}") │
│   • wwwroot/print.css            (estilos de impresión)     │
│   • store-api: GET /api/orders/{id}/pdf                     │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│  4. NOTIFICACIÓN POR EMAIL           Estado: pendiente 🔲   │
│                                                             │
│  Al crear la orden, store-api envía:                        │
│   • Email al comprador con resumen + PDF adjunto            │
│   • Email al vendedor con detalle del pedido                │
│                                                             │
│  Opciones de implementación:                                │
│   • SendGrid (plan gratuito disponible)                     │
│   • System.Net.Mail con SMTP Gmail/Outlook (desarrollo)     │
│   • MailKit (más robusto para producción)                   │
│                                                             │
│  Archivos a crear en store-api:                             │
│   • Services/EmailService.cs                                │
│   • Templates/OrderConfirmation.html                        │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│  5. SEGUIMIENTO DEL PEDIDO           Estado: pendiente 🔲   │
│                                                             │
│  Requiere autenticación de usuarios. Incluye:               │
│   • Página /mis-ordenes con historial                       │
│   • Estados: Pendiente → En preparación → Enviado →         │
│              Entregado → Cancelado                          │
│   • Notificación de cambio de estado por email              │
│   • Opción de cancelar orden (si está en estado Pendiente)  │
│                                                             │
│  Archivos a crear:                                          │
│   • Pages/MyOrders.razor                                    │
│   • Pages/OrderTracking.razor                               │
│   • store-api: GET /api/orders/user/{userId}                │
│   • store-api: PUT /api/orders/{id}/status                  │
│   • store-api: DELETE /api/orders/{id} (cancelar)           │
└─────────────────────────────────────────────────────────────┘
```

### Prioridad de implementación recomendada

| Paso | Esfuerzo | Valor | Dependencias |
|---|---|---|---|
| 3. Comprobante imprimible | Bajo — solo Razor + CSS | Alto — el usuario lo espera | Ninguna |
| 4. Email de confirmación | Medio — requiere SMTP | Alto — profesionaliza la app | Paso 3 (para adjuntar) |
| 5. Seguimiento | Alto — requiere auth + BD | Medio — mejora la experiencia | Autenticación |

---

## 🔍 Búsqueda de Productos

La búsqueda se maneja por query string, sin llamadas adicionales a la API:

```
Usuario escribe en NavBar → navega a /products?q=término
        │
        ▼
Products.razor lee ?q= en OnParametersSet()
        │
        ▼
Filtra _todos (lista completa ya cargada) en memoria
        │
        ▼
Muestra _filtrados con contador de resultados
```

---

## ⚙️ Configuración

### `Program.cs`

```csharp
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CartState>();
builder.Services.AddScoped<CartStorage>();
builder.Services.AddScoped<CartService>();

builder.Services.AddHttpClient<ProductService>(client =>
    client.BaseAddress = new Uri("https://localhost:7179/"));

builder.Services.AddHttpClient<OrderService>(client =>
    client.BaseAddress = new Uri("https://localhost:7179/"));

// Necesario para listas de productos con imágenes (supera el límite por defecto de 32KB)
builder.Services.AddSignalR(e =>
    e.MaximumReceiveMessageSize = 102400000);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => options.DetailedErrors = true);
```

### `App.razor`

```razor
<!-- prerender: false es obligatorio — LocalStorage requiere JS interop -->
<HeadOutlet @rendermode="@(new InteractiveServerRenderMode(prerender: false))" />
<Routes @rendermode="@(new InteractiveServerRenderMode(prerender: false))" />

<!-- blazor.web.js — correcto para Blazor Web App (.NET 8) -->
<script src="_framework/blazor.web.js"></script>
```

---

## 🚀 Ejecución

### Requisitos

- .NET 8 SDK
- `store-api` corriendo en `https://localhost:7179`

### Pasos

```bash
# 1. Clonar
git clone https://github.com/tu-usuario/shop_cart.git
cd shop_cart

# 2. Restaurar dependencias
dotnet restore

# 3. Asegurarse de que store-api está corriendo

# 4. Ejecutar
dotnet run
```

La app queda disponible en `https://localhost:7153`.

---

## 📦 Dependencias NuGet

| Paquete | Uso |
|---|---|
| `Blazored.LocalStorage` | Persistencia del carrito en el navegador |
| `QRCoder` | Generación de código QR para método de pago QR |

---

## ⚠️ Decisiones técnicas

### `prerender: false`
`Blazored.LocalStorage` usa JS interop que no está disponible durante el prerendering del servidor. Se desactiva globalmente en `App.razor` para evitar excepciones de tipo `InvalidOperationException: JavaScript interop calls cannot be issued at this time`.

### `blazor.web.js` en lugar de `blazor.server.js`
Este proyecto es **Blazor Web App (.NET 8)**, no Blazor Server clásico. Usar `blazor.server.js` causa desconexiones inmediatas del circuito SignalR.

### Límite de SignalR ampliado
El límite por defecto de SignalR es 32KB. Las listas de productos con imágenes superan ese límite, causando que el circuito se cierre con el error `Connection closed with an error`. Se amplía a 100MB en `Program.cs`.

### `@rendermode` solo en el componente raíz
En Blazor Web App, el `@rendermode InteractiveServer` se declara **una sola vez** en el componente padre de la jerarquía. Los componentes hijos como `ProductCard` lo heredan automáticamente — declararlo en el hijo causa conflictos que crashean el circuito.

---

## 🚧 Mejoras futuras

### Próximas (sin dependencias externas)
- [ ] Página `/orden/{orderId}` — comprobante imprimible con CSS `@media print`
- [ ] Botón "Descargar PDF" desde `store-api GET /api/orders/{id}/pdf`
- [ ] QR de verificación en el comprobante
- [ ] Paginación en la lista de productos
- [ ] Optimizar payload — cargar imágenes por URL en lugar de base64

### Requieren servicio externo
- [ ] Email de confirmación al comprador (SendGrid / MailKit)
- [ ] Email de aviso al vendedor por nueva orden
- [ ] Integración con pasarela de pago real (Stripe, MercadoPago)

### Requieren autenticación
- [ ] Registro e inicio de sesión de usuarios
- [ ] Historial de órdenes por usuario (`/mis-ordenes`)
- [ ] Seguimiento de estado del pedido
- [ ] Cancelación de órdenes en estado Pendiente

### Arquitectura
- [ ] Proyecto compartido para modelos entre Blazor y API
- [ ] Tests unitarios de `CartState` y `OrderService`

---

## 🔗 Proyectos relacionados

| Proyecto | Descripción | Repositorio |
|---|---|---|
| `store-api` | API REST que provee productos y procesa órdenes | [ver README](../store-api/README.md) |
