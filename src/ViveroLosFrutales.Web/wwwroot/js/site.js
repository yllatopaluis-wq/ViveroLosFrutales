window.viveroDetalleRows = function () {
  const table = document.querySelector("#detalle tbody");
  if (!table) return;

  table.addEventListener("click", function (event) {
    if (!event.target.classList.contains("add-row")) return;
    const index = table.querySelectorAll("tr").length;
    const row = document.createElement("tr");
    row.innerHTML = `
      <td><input name="Detalles[${index}].ProductoId" class="form-control form-control-sm" type="number" /></td>
      <td><input name="Detalles[${index}].Cantidad" class="form-control form-control-sm text-end" type="number" step="0.01" /></td>
      <td><input name="Detalles[${index}].PrecioUnitario" class="form-control form-control-sm text-end" type="number" step="0.01" /></td>
      <td><button type="button" class="btn btn-sm btn-outline-danger remove-row">Quitar</button></td>`;
    table.appendChild(row);
  });

  table.addEventListener("click", function (event) {
    if (!event.target.classList.contains("remove-row")) return;
    event.target.closest("tr").remove();
  });
};


window.viveroFiltrarCuentasPorMedioPago = function (config) {
  const root = config?.rootSelector ? document.querySelector(config.rootSelector) : document;
  if (!root) return;

  const medioPago = root.querySelector(config?.medioPagoSelector || '[name="MedioPago"]');
  const cuenta = root.querySelector(config?.cuentaSelector || '[name="CuentaFinancieraId"]');
  if (!medioPago || !cuenta) return;

  function normalizar(value) {
    return String(value || '').trim().toUpperCase();
  }

  function seleccionarPrimeraCuentaVisible() {
    const primera = Array.from(cuenta.options).find((option) => !option.disabled && !option.hidden);
    if (primera) cuenta.value = primera.value;
  }

  function sincronizar() {
    const esEfectivo = normalizar(medioPago.value) === 'EFECTIVO';
    let seleccionOculta = false;

    Array.from(cuenta.options).forEach((option) => {
      const tipo = normalizar(option.dataset.tipo);
      const visible = esEfectivo
        ? option.value === '' || tipo === 'CAJA'
        : tipo === 'BANCO';
      option.hidden = !visible;
      option.disabled = !visible;
      if (!visible && option.selected) seleccionOculta = true;
    });

    if (seleccionOculta) seleccionarPrimeraCuentaVisible();
  }

  medioPago.addEventListener('change', sincronizar);
  medioPago.addEventListener('input', sincronizar);
  sincronizar();
};

window.viveroComprobanteForm = function (config) {
  const clienteSearch = document.querySelector("#clienteSearch");
  const clienteId = document.querySelector("#clienteId");
  const clienteDireccion = document.querySelector("#clienteDireccion");
  const clienteResumenNombre = document.querySelector("#clienteResumenNombre");
  const clienteResumenDocumento = document.querySelector("#clienteResumenDocumento");
  const clientesOptions = document.querySelectorAll("#clientesOptions option");
  let clientes = config.clientes || [];
  const tipoComprobante = document.querySelector("#tipoComprobante");
  const serie = document.querySelector("#serieComprobante");
  const correlativo = document.querySelector("#correlativoComprobante");
  const detalleTable = document.querySelector("#detalle");
  const detalleBody = document.querySelector("#detalle tbody");
  const comprobanteSubtotal = document.querySelector("#comprobanteSubtotal");
  const comprobanteSubtotalExonerado = document.querySelector("#comprobanteSubtotalExonerado");
  const comprobanteSubtotalGravado = document.querySelector("#comprobanteSubtotalGravado");
  const comprobanteIgv = document.querySelector("#comprobanteIgv");
  const comprobanteTotal = document.querySelector("#comprobanteTotal");
  let productos = config.productos || [];
  const showTotals = detalleTable?.dataset.showTotals === "true";
  const externalProductPicker = detalleTable?.dataset.productPickerOwned === "true";

  function escapeHtml(value) {
    return String(value ?? "")
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }

  async function buscarClientesRemoto(term) {
    if (!config.clientesUrl) return clientes;
    const url = `${config.clientesUrl}?search=${encodeURIComponent(term || "")}`;
    const response = await fetch(url, { headers: { "Accept": "application/json" } });
    if (!response.ok) return clientes;
    clientes = await response.json();
    return clientes;
  }

  function pintarClienteResumen(cliente) {
    if (clienteResumenNombre) clienteResumenNombre.textContent = cliente?.nombre || "Seleccione cliente";
    if (clienteResumenDocumento) clienteResumenDocumento.textContent = cliente?.documento || "-";
    if (clienteResumenTelefono) clienteResumenTelefono.textContent = cliente?.telefono || "-";
    if (clienteResumenEmail) clienteResumenEmail.textContent = cliente?.email || "-";
    if (clienteResumenDireccion) clienteResumenDireccion.textContent = cliente?.direccion || "-";
    if (clienteDireccion) clienteDireccion.value = cliente?.direccion || "";
  }

  function resolverCliente() {
    if (!clienteSearch || !clienteId) return;
    const cliente = clientes.find((item) => item.texto === clienteSearch.value);
    const option = Array.from(clientesOptions).find((item) => item.value === clienteSearch.value);
    clienteId.value = cliente ? cliente.id : option ? option.dataset.id : "";
    if (cliente) {
      pintarClienteResumen(cliente);
    } else if (option) {
      pintarClienteResumen({
        nombre: option.dataset.nombre || option.value.split(" - ")[0] || "Seleccione cliente",
        documento: option.dataset.documento || option.value.split(" - ").slice(1).join(" - ") || "-",
        telefono: option.dataset.telefono || "",
        email: option.dataset.email || "",
        direccion: option.dataset.direccion || ""
      });
    } else {
      pintarClienteResumen(null);
    }
  }

  function configurarClienteBuscable() {
    if (!clienteSearch || !clienteId || !config.clientesUrl) return;
    let results = document.querySelector(".cliente-search-results");
    if (!results) {
      results = document.createElement("div");
      results.className = "producto-search-results cliente-search-results";
      results.hidden = true;
      document.body.appendChild(results);
    }
    let timer;
    let clienteSearchRequest = 0;

    function cerrarResultados() {
      clienteSearchRequest++;
      results.hidden = true;
    }

    function pintarResultados(items) {
      const rect = clienteSearch.getBoundingClientRect();
      results.style.left = `${rect.left}px`;
      results.style.top = `${rect.bottom + 4}px`;
      results.style.width = `${rect.width}px`;
      results.innerHTML = items.length
        ? items.map((cliente) => `<button type="button" class="producto-search-option cliente-search-option" data-id="${escapeHtml(cliente.id)}">${escapeHtml(cliente.texto)}</button>`).join("")
        : '<div class="producto-search-empty">Sin coincidencias</div>';
      results.hidden = false;
    }

    async function actualizar() {
      const requestId = ++clienteSearchRequest;
      const items = await buscarClientesRemoto(clienteSearch.value);
      if (requestId !== clienteSearchRequest) return;
      pintarResultados(items);
      resolverCliente();
    }

    clienteSearch.addEventListener("input", function () {
      window.clearTimeout(timer);
      timer = window.setTimeout(actualizar, 180);
    });
    clienteSearch.addEventListener("focus", actualizar);
    clienteSearch.addEventListener("blur", function () {
      resolverCliente();
      window.setTimeout(cerrarResultados, 150);
    });
    results.addEventListener("mousedown", function (event) {
      event.preventDefault();
      const option = event.target.closest(".cliente-search-option");
      if (!option) return;
      const cliente = clientes.find((item) => String(item.id) === String(option.dataset.id));
      if (!cliente) return;
      clienteSearch.value = cliente.texto || "";
      clienteId.value = cliente.id;
      pintarClienteResumen(cliente);
      cerrarResultados();
    });
  }

  async function actualizarNumeracion() {
    if (!tipoComprobante || !serie || !correlativo || !config.numeracionUrl) return;
    const url = `${config.numeracionUrl}?tipoComprobante=${encodeURIComponent(tipoComprobante.value)}`;
    const response = await fetch(url, { headers: { "Accept": "application/json" } });
    if (!response.ok) return;

    const data = await response.json();
    serie.value = data.serie || data.Serie || "";
    correlativo.value = data.correlativo || data.Correlativo || "";
    const numero = document.querySelector(".quote-number");
    if (numero) numero.textContent = `${serie.value}-${correlativo.value}`;
  }

  function productoOptions(selectedId) {
    const options = ['<option value="">Seleccione producto</option>'];
    for (const producto of productos) {
      const selected = String(producto.id) === String(selectedId) ? " selected" : "";
      const precio = precioProducto(producto);
      const afectoIgv = producto.afectoIgv === true || producto.AfectoIgv === true;
      options.push(`<option value="${producto.id}" data-precio="${precio}" data-afecto-igv="${afectoIgv}"${selected}>${escapeHtml(producto.texto)}</option>`);
    }
    return options.join("");
  }

  function productoSearchMarkup(name, selectedId) {
    const producto = obtenerProducto(selectedId);
    const texto = producto?.texto || "";
    return `
      <div class="producto-search-wrap">
        <input class="form-control form-control-sm producto-search" value="${escapeHtml(texto)}" placeholder="Buscar producto" autocomplete="off" />
        <button type="button" class="producto-search-toggle" aria-label="Mostrar productos">v</button>
        <div class="producto-search-results" hidden></div>
      </div>
      <input name="${escapeHtml(name)}" class="producto-select" type="hidden" value="${escapeHtml(selectedId || "")}" />`;
  }

  function formatMoney(value) {
    return `S/ ${Number(value || 0).toFixed(2)}`;
  }

  function precioProducto(producto) {
    if (!producto) return 0;
    const precio = producto.precio ?? producto.Precio ?? producto.precioVentaConIgv ?? producto.PrecioVentaConIgv ?? 0;
    if (Number(precio) > 0) return Number(precio);
    const precioSinIgv = producto.precioVentaSinIgv ?? producto.PrecioVentaSinIgv ?? 0;
    const afectoIgv = producto.afectoIgv === true || producto.AfectoIgv === true;
    return afectoIgv ? Math.round(Number(precioSinIgv || 0) * 1.18 * 100) / 100 : Number(precioSinIgv || 0);
  }

  function obtenerProducto(productoId) {
    return productos.find((producto) => String(producto.id) === String(productoId));
  }

  function obtenerProductoPorTexto(texto) {
    const normalizado = String(texto || "").trim().toLocaleUpperCase();
    if (!normalizado) return undefined;
    return productos.find((producto) => String(producto.texto || "").trim().toLocaleUpperCase() === normalizado);
  }

  function normalizarBusqueda(value) {
    return String(value || "")
      .normalize("NFD")
      .replace(/[\u0300-\u036f]/g, "")
      .trim()
      .toLocaleUpperCase();
  }

  function buscarProductos(term) {
    const query = normalizarBusqueda(term);
    if (!query) return productos.slice(0, 50);

    return productos
      .map((producto) => {
        const texto = normalizarBusqueda(producto.texto);
        const nombre = normalizarBusqueda(producto.nombre);
        const categoria = normalizarBusqueda(producto.categoria);
        const textoIndex = texto.indexOf(query);
        const nombreIndex = nombre.indexOf(query);
        const categoriaIndex = categoria.indexOf(query);
        const wordStart = texto.split(/\s+/).some((word) => word.startsWith(query));
        if (textoIndex < 0 && nombreIndex < 0 && categoriaIndex < 0) return null;

        let score = 5;
        if (texto === query || nombre === query) score = 0;
        else if (texto.startsWith(query) || nombre.startsWith(query)) score = 1;
        else if (wordStart) score = 2;
        else if (nombreIndex >= 0) score = 3;
        else if (textoIndex >= 0) score = 4;

        return { producto, score, index: Math.min(...[textoIndex, nombreIndex, categoriaIndex].filter((x) => x >= 0)) };
      })
      .filter(Boolean)
      .sort((a, b) => a.score - b.score || a.index - b.index || String(a.producto.texto).localeCompare(String(b.producto.texto)))
      .slice(0, 50)
      .map((item) => item.producto);
  }

  async function buscarProductosRemoto(term) {
    if (!config.productosUrl) return buscarProductos(term);
    const url = `${config.productosUrl}?search=${encodeURIComponent(term || "")}`;
    const response = await fetch(url, { headers: { "Accept": "application/json" } });
    if (!response.ok) return buscarProductos(term);
    const encontrados = await response.json();
    const map = new Map(productos.map((producto) => [String(producto.id), producto]));
    for (const producto of encontrados) {
      map.set(String(producto.id), producto);
    }
    productos = Array.from(map.values());
    return encontrados;
  }

  function configurarProductoBuscable(row) {
    const currentSelect = row.querySelector("select.producto-select");
    if (currentSelect) {
      const name = currentSelect.getAttribute("name") || "";
      const selectedId = currentSelect.value || "";
      const cell = currentSelect.closest("td");
      if (!cell) return;
      cell.innerHTML = productoSearchMarkup(name, selectedId);
    }

    const search = row.querySelector(".producto-search");
    const results = row.querySelector(".producto-search-results");
    const toggle = row.querySelector(".producto-search-toggle");
    const hidden = row.querySelector('input.producto-select[type="hidden"]');
    if (!search || !results || !hidden) return;
    if (results.parentElement !== document.body) {
      document.body.appendChild(results);
    }

    let punteroEnResultados = false;

    function cerrarResultados() {
      results.hidden = true;
    }

    function cerrarResultadosPorScroll(event) {
      if (event?.target && results.contains(event.target)) return;
      cerrarResultados();
    }

    function seleccionarProducto(producto) {
      search.value = producto.texto || "";
      hidden.value = producto.id;
      cerrarResultados();
      hidden.dispatchEvent(new Event("change", { bubbles: true }));
    }

    async function mostrarResultados() {
      const encontrados = await buscarProductosRemoto(search.value);
      const rect = search.getBoundingClientRect();
      results.style.left = `${rect.left}px`;
      results.style.top = `${rect.bottom + 4}px`;
      results.style.width = `${rect.width}px`;
      results.innerHTML = encontrados.length
        ? encontrados
            .map((producto) => `<button type="button" class="producto-search-option" data-id="${escapeHtml(producto.id)}">${escapeHtml(producto.texto)}</button>`)
            .join("")
        : '<div class="producto-search-empty">Sin coincidencias</div>';
      results.hidden = false;
    }

    function resolverProducto() {
      const producto = obtenerProductoPorTexto(search.value);
      hidden.value = producto ? producto.id : "";
      hidden.dispatchEvent(new Event("change", { bubbles: true }));
    }

    search.addEventListener("input", function () {
      resolverProducto();
      mostrarResultados();
    });
    search.addEventListener("focus", mostrarResultados);
    search.addEventListener("change", resolverProducto);
    search.addEventListener("blur", function () {
      resolverProducto();
      window.setTimeout(function () {
        if (!punteroEnResultados) cerrarResultados();
      }, 150);
    });
    toggle?.addEventListener("click", function () {
      search.focus();
      mostrarResultados();
    });
    toggle?.addEventListener("blur", function () {
      window.setTimeout(function () {
        if (!punteroEnResultados && document.activeElement !== search) cerrarResultados();
      }, 150);
    });
    window.addEventListener("scroll", cerrarResultadosPorScroll, true);
    window.addEventListener("resize", cerrarResultados);
    results.addEventListener("pointerenter", function () {
      punteroEnResultados = true;
    });
    results.addEventListener("pointerleave", function () {
      punteroEnResultados = false;
      if (document.activeElement !== search) cerrarResultados();
    });
    results.addEventListener("mousedown", function (event) {
      const option = event.target.closest(".producto-search-option");
      if (!option) return;
      event.preventDefault();
      const producto = obtenerProducto(option.dataset.id);
      if (producto) seleccionarProducto(producto);
    });
  }

  function recalcularTotalesComprobante() {
    if (!detalleBody || !comprobanteIgv || !comprobanteTotal) return;
    let subtotalExonerado = 0;
    let subtotalGravado = 0;
    let igv = 0;

    detalleBody.querySelectorAll("tr").forEach((row) => {
      const select = row.querySelector(".producto-select");
      const cantidad = Number(row.querySelector(".cantidad-input")?.value || 0);
      const precio = Number(row.querySelector(".precio-input")?.value || 0);
      const importe = cantidad * precio;
      if (importe <= 0) return;

      const producto = obtenerProducto(select?.value);
      const afectoIgv = producto?.afectoIgv === true || producto?.AfectoIgv === true;
      if (afectoIgv) {
        const baseGravada = Math.round((importe / 1.18) * 100) / 100;
        subtotalGravado += baseGravada;
        igv += importe - baseGravada;
      } else {
        subtotalExonerado += importe;
      }
    });

    const subtotal = subtotalExonerado + subtotalGravado;
    if (comprobanteSubtotal) comprobanteSubtotal.textContent = formatMoney(subtotalGravado);
    if (comprobanteSubtotalExonerado) comprobanteSubtotalExonerado.textContent = formatMoney(subtotalExonerado);
    if (comprobanteSubtotalGravado) comprobanteSubtotalGravado.textContent = formatMoney(subtotalGravado);
    comprobanteIgv.textContent = formatMoney(igv);
    comprobanteTotal.textContent = formatMoney(subtotal + igv);
  }

  function configurarFila(row) {
    configurarProductoBuscable(row);
    const select = row.querySelector(".producto-select");
    const precio = row.querySelector(".precio-input");
    const cantidad = row.querySelector(".cantidad-input");
    const total = row.querySelector(".line-total");
    if (!select || !precio) return;

    function recalcularTotal() {
      if (!total) return;
      const cantidadValue = Number(cantidad?.value || 0);
      const precioValue = Number(precio.value || 0);
      total.value = (cantidadValue * precioValue).toFixed(2);
      recalcularTotalesComprobante();
    }

    select.addEventListener("change", function () {
      const producto = obtenerProducto(select.value);
      if (producto) {
        precio.value = precioProducto(producto).toFixed(2);
      }
      recalcularTotal();
    });
    if (select.value && Number(precio.value || 0) <= 0) {
      const producto = obtenerProducto(select.value);
      if (producto) precio.value = precioProducto(producto).toFixed(2);
    }
    cantidad?.addEventListener("input", recalcularTotal);
    precio.addEventListener("input", recalcularTotal);
    recalcularTotal();
  }

  if (clienteSearch) {
    configurarClienteBuscable();
    clienteSearch.addEventListener("change", resolverCliente);
    clienteSearch.addEventListener("blur", resolverCliente);
  }

  if (tipoComprobante) {
    tipoComprobante.addEventListener("change", actualizarNumeracion);
  }

  if (detalleBody && !externalProductPicker) {
    detalleBody.querySelectorAll("tr").forEach(configurarFila);
    detalleBody.addEventListener("click", function (event) {
      const removeButton = event.target.closest(".remove-row");
      if (removeButton) {
        removeButton.closest("tr").remove();
        recalcularTotalesComprobante();
        return;
      }

      const addButton = event.target.closest(".add-row");
      if (!addButton) return;
      const index = detalleBody.querySelectorAll("tr").length;
      const row = document.createElement("tr");
      row.innerHTML = showTotals
        ? `
          <td>${productoSearchMarkup(`Detalles[${index}].ProductoId`, "")}</td>
          <td><input value="UNIDAD" class="form-control form-control-sm" readonly /></td>
          <td><input name="Detalles[${index}].Cantidad" class="form-control form-control-sm text-end cantidad-input" type="number" step="0.01" min="0.01" /></td>
          <td><input name="Detalles[${index}].PrecioUnitario" class="form-control form-control-sm text-end precio-input" type="number" step="0.01" min="0" /></td>
          <td><input value="0.00" class="form-control form-control-sm text-end line-total" readonly /></td>
          <td class="text-center">
            <button type="button" class="icon-btn danger remove-row" title="Quitar fila" aria-label="Quitar fila">
              <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M18 6 6 18" /><path d="m6 6 12 12" /></svg>
            </button>
          </td>`
        : `
          <td>${productoSearchMarkup(`Detalles[${index}].ProductoId`, "")}</td>
          <td><input name="Detalles[${index}].Cantidad" class="form-control form-control-sm text-end cantidad-input" type="number" step="0.01" min="0.01" /></td>
          <td><input name="Detalles[${index}].PrecioUnitario" class="form-control form-control-sm text-end precio-input" type="number" step="0.01" min="0" /></td>
          <td><button type="button" class="btn btn-sm btn-outline-danger remove-row">Quitar</button></td>`;
      detalleBody.appendChild(row);
      configurarFila(row);
    });
  }
};

window.viveroProductoGridSearch = function (config) {
  const table = document.querySelector(config?.tableSelector || "#detalle");
  const body = table?.querySelector("tbody");
  const picker = document.querySelector(config?.pickerSelector || "[data-product-grid-picker]");
  if (!table || !body || !picker) return;

  let productos = config?.productos || [];
  const search = picker.querySelector(".producto-grid-search");
  const results = picker.querySelector(".producto-search-results");
  const toggle = picker.querySelector(".producto-search-toggle");
  const add = picker.querySelector(".product-grid-add");
  const subtotalExonerado = document.querySelector("#comprobanteSubtotalExonerado");
  const subtotalGravado = document.querySelector("#comprobanteSubtotalGravado");
  const descuentoOutput = document.querySelector("#comprobanteDescuento");
  const igvOutput = document.querySelector("#comprobanteIgv");
  const totalOutput = document.querySelector("#comprobanteTotal");
  if (!search || !results || !add) return;
  if (results.parentElement !== document.body) document.body.appendChild(results);

  const gridColumns = Array.isArray(config?.columns) && config.columns.length ? config.columns : ["Producto", "Unidad", "Stock", "Cantidad", "PrecioUnitario", "DescuentoPorcentaje", "TotalLinea"];
  const productBehavior = config?.behavior || {};
  const priceFieldName = config?.priceFieldName || "PrecioUnitario";
  const unitFieldName = config?.unitFieldName || null;
  const mostrarItemProducto = productBehavior.mostrarItem !== false && productBehavior.MostrarItem !== false;
  const mostrarAccionProducto = productBehavior.mostrarAccion !== false && productBehavior.MostrarAccion !== false;
  const cantidadInicialProducto = Math.max(Number(productBehavior.cantidadInicial || productBehavior.CantidadInicial || 1), 0.01);
  let itemsActuales = [];
  let indiceActivo = -1;
  let punteroEnResultados = false;

  function escapeHtml(value) {
    return String(value ?? "")
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }

  function normalizar(value) {
    return String(value || "")
      .normalize("NFD")
      .replace(/[\u0300-\u036f]/g, "")
      .trim()
      .toLocaleUpperCase();
  }

  function codigoProducto(producto) {
    return producto?.codigo || producto?.Codigo || (producto?.id ? `PROD-${String(producto.id).padStart(6, "0")}` : "");
  }

  function skuProducto(producto) {
    return producto?.sku || producto?.Sku || "";
  }

  function codigoBarrasProducto(producto) {
    return producto?.codigoBarras || producto?.CodigoBarras || "";
  }

  function unidadProducto(producto) {
    return producto?.unidad || producto?.Unidad || producto?.unidadMedida || producto?.UnidadMedida || "NIU";
  }

  function precioProducto(producto) {
    return Number(producto?.precio ?? producto?.Precio ?? producto?.precioVentaConIgv ?? producto?.PrecioVentaConIgv ?? 0);
  }

  function stockProducto(producto) {
    return Number(producto?.stock ?? producto?.Stock ?? 0);
  }

  function esAfecto(producto) {
    return producto?.afectoIgv === true || producto?.AfectoIgv === true;
  }

  function textoProducto(producto) {
    if (!producto) return "";
    return producto.texto || producto.Texto || producto.nombre || producto.Nombre || "";
  }

  function formatMoney(value) {
    return `S/ ${Number(value || 0).toFixed(2)}`;
  }

  function productoIdDeFila(row) {
    const value = row?.querySelector(".producto-select")?.value || "";
    return value === "0" ? "" : value;
  }

  function filaSinProducto(row) {
    return !productoIdDeFila(row);
  }

  function obtenerProducto(productoId) {
    return productos.find((producto) => String(producto.id) === String(productoId));
  }

  function buscarLocal(term) {
    const query = normalizar(term);
    if (!query) return productos.slice(0, 25);
    return productos.filter((producto) => {
      return normalizar(textoProducto(producto)).includes(query)
        || normalizar(producto.nombre || producto.Nombre).includes(query)
        || normalizar(producto.categoria || producto.Categoria).includes(query)
        || normalizar(codigoProducto(producto)).includes(query)
        || normalizar(skuProducto(producto)).includes(query)
        || normalizar(codigoBarrasProducto(producto)).includes(query);
    }).slice(0, 25);
  }

  async function buscarRemoto(term) {
    if (!config?.productosUrl) return buscarLocal(term);
    const response = await fetch(`${config.productosUrl}?search=${encodeURIComponent(term || "")}`, { headers: { "Accept": "application/json" } });
    if (!response.ok) return buscarLocal(term);
    const encontrados = await response.json();
    const map = new Map(productos.map((producto) => [String(producto.id), producto]));
    encontrados.forEach((producto) => map.set(String(producto.id), producto));
    productos = Array.from(map.values());
    return encontrados;
  }

  function cerrarResultados() {
    results.hidden = true;
    indiceActivo = -1;
  }

  function cerrarResultadosPorScroll(event) {
    if (event?.target && results.contains(event.target)) return;
    cerrarResultados();
  }
  function renumerar() {
    body.querySelectorAll("tr").forEach((row, index) => {
      const number = row.querySelector(".quote-row-number");
      if (number) number.textContent = String(index + 1);
      row.querySelector(".producto-select")?.setAttribute("name", `Detalles[${index}].ProductoId`);
      row.querySelector(".cantidad-input")?.setAttribute("name", `Detalles[${index}].Cantidad`);
      if (unitFieldName) row.querySelector(".unidad-input")?.setAttribute("name", `Detalles[${index}].${unitFieldName}`);
      row.querySelector(".precio-input")?.setAttribute("name", `Detalles[${index}].${priceFieldName}`);
    });
  }

  function marcarStock(row) {
    const cantidad = Number(row.querySelector(".cantidad-input")?.value || 0);
    const stock = Number(row.querySelector(".stock-input")?.value || 0);
    row.classList.toggle("stock-warning", stock > 0 && cantidad > stock);
  }

  function recalcular() {
    let exonerado = 0;
    let gravado = 0;
    let igv = 0;
    let descuentoTotal = 0;
    body.querySelectorAll("tr").forEach((row) => {
      const producto = obtenerProducto(productoIdDeFila(row));
      const cantidadInput = row.querySelector(".cantidad-input");
      const precio = Number(row.querySelector(".precio-input")?.value || 0);
      const descuento = Math.min(Math.max(Number(row.querySelector(".descuento-input")?.value || 0), 0), 100);
      const cantidad = Math.max(Number(cantidadInput?.value || 0), 0);
      if (cantidadInput && Number(cantidadInput.value || 0) <= 0) row.classList.add("quantity-warning");
      else row.classList.remove("quantity-warning");
      const bruto = cantidad * precio;
      const importe = bruto * (1 - descuento / 100);
      descuentoTotal += bruto - importe;
      const total = row.querySelector(".line-total");
      if (total) total.value = importe.toFixed(2);
      marcarStock(row);
      if (importe <= 0) return;
      if (esAfecto(producto)) {
        const base = Math.round((importe / 1.18) * 100) / 100;
        gravado += base;
        igv += importe - base;
      } else {
        exonerado += importe;
      }
    });
    if (subtotalExonerado) subtotalExonerado.textContent = formatMoney(exonerado);
    if (subtotalGravado) subtotalGravado.textContent = formatMoney(gravado);
    if (descuentoOutput) descuentoOutput.textContent = formatMoney(descuentoTotal);
    if (igvOutput) igvOutput.textContent = formatMoney(igv);
    if (totalOutput) totalOutput.textContent = formatMoney(exonerado + gravado + igv);
  }

  function celdaProducto(field, index, producto) {
    const cantidadInicial = cantidadInicialProducto;
    const precioReadonly = productBehavior.permitirEditarPrecio === false || productBehavior.PermitirEditarPrecio === false ? " readonly" : "";
    const descuentoReadonly = productBehavior.permitirDescuento === false || productBehavior.PermitirDescuento === false ? " readonly" : "";
    switch (field) {
      case "Codigo":
        return `<td data-field="Codigo"><input value="${escapeHtml(codigoProducto(producto))}" class="form-control form-control-sm codigo-input" readonly /></td>`;
      case "Producto":
        return `<td data-field="Producto"><input class="form-control form-control-sm producto-search" value="${escapeHtml(textoProducto(producto))}" readonly /><input name="Detalles[${index}].ProductoId" class="producto-select" type="hidden" value="${escapeHtml(producto?.id || "")}" /></td>`;
      case "Unidad":
        return `<td data-field="Unidad"><input ${unitFieldName ? `name="Detalles[${index}].${unitFieldName}"` : ""} value="${escapeHtml(unidadProducto(producto))}" class="form-control form-control-sm unidad-input" readonly /></td>`;
      case "Stock":
        return `<td data-field="Stock"><input value="${stockProducto(producto).toFixed(2)}" class="form-control form-control-sm text-end stock-input" readonly /></td>`;
      case "Cantidad":
        return `<td data-field="Cantidad"><input name="Detalles[${index}].Cantidad" value="${cantidadInicial}" class="form-control form-control-sm text-end cantidad-input" type="number" step="0.01" min="0.01" /></td>`;
      case "PrecioUnitario":
        return `<td data-field="PrecioUnitario"><input name="Detalles[${index}].${priceFieldName}" value="${precioProducto(producto).toFixed(2)}" class="form-control form-control-sm text-end precio-input" type="number" step="0.01" min="0"${precioReadonly} /></td>`;
      case "DescuentoPorcentaje":
        return `<td data-field="DescuentoPorcentaje"><input value="0" class="form-control form-control-sm text-end descuento-input" type="number" step="0.01" min="0" max="100"${descuentoReadonly} /></td>`;
      case "TotalLinea":
        return `<td data-field="TotalLinea"><input value="0.00" class="form-control form-control-sm text-end line-total" readonly /></td>`;
      default:
        return "";
    }
  }

  function crearFila(producto) {
    const index = body.querySelectorAll("tr").length;
    const row = document.createElement("tr");
    row.innerHTML = `
      ${mostrarItemProducto ? `<td class="quote-row-number">${index + 1}</td>` : ""}
      ${gridColumns.map((field) => celdaProducto(field, index, producto)).join("")}
      ${mostrarAccionProducto ? `<td class="text-center"><button type="button" class="icon-btn danger remove-row" title="Quitar fila" aria-label="Quitar fila"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M18 6 6 18" /><path d="m6 6 12 12" /></svg></button></td>` : ""}`;
    body.appendChild(row);
    enlazarFila(row);
    return row;
  }
  function aplicarProducto(row, producto) {
    if (!row || !producto) return;
    const productoSelect = row.querySelector(".producto-select");
    if (productoSelect) productoSelect.value = producto.id;
    const productoSearch = row.querySelector(".producto-search");
    if (productoSearch) productoSearch.value = textoProducto(producto);
    const unidadInput = row.querySelector(".unidad-input");
    if (unidadInput) unidadInput.value = unidadProducto(producto);
    const stockInput = row.querySelector(".stock-input");
    if (stockInput) stockInput.value = stockProducto(producto).toFixed(2);
    const cantidad = row.querySelector(".cantidad-input");
    const precio = row.querySelector(".precio-input");
    const descuento = row.querySelector(".descuento-input");
    if (cantidad && Number(cantidad.value || 0) <= 0) cantidad.value = String(cantidadInicialProducto).replace(/\.00$/, "");
    if (precio && !precio.readOnly) precio.value = precioProducto(producto).toFixed(2);
    else if (precio) precio.value = precioProducto(producto).toFixed(2);
    if (descuento && descuento.value === "") descuento.value = "0";
    recalcular();
  }

  function agregarProducto(producto) {
    if (!producto) return;
    const unirDuplicados = productBehavior.unirProductosDuplicados !== false && productBehavior.UnirProductosDuplicados !== false;
    let row = unirDuplicados ? Array.from(body.querySelectorAll("tr")).find((item) => String(productoIdDeFila(item)) === String(producto.id)) : null;
    if (row) {
      const cantidad = row.querySelector(".cantidad-input");
      cantidad.value = (Number(cantidad.value || 0) + 1).toFixed(2).replace(/\.00$/, "");
      row.classList.add("product-merged");
      window.setTimeout(() => row.classList.remove("product-merged"), 550);
    } else {
      row = Array.from(body.querySelectorAll("tr")).find(filaSinProducto);
      if (!row) row = crearFila(producto);
      aplicarProducto(row, producto);
    }
    renumerar();
    recalcular();
  }

  function enlazarFila(row) {
    if (row.dataset.productGridBound === "true") return;
    row.dataset.productGridBound = "true";
    row.querySelector(".cantidad-input")?.addEventListener("input", recalcular);
    row.querySelector(".precio-input")?.addEventListener("input", recalcular);
    row.querySelector(".descuento-input")?.addEventListener("input", recalcular);
    row.querySelector(".producto-select")?.addEventListener("change", recalcular);
  }

  function setActiveIndex(index) {
    if (itemsActuales.length === 0) return;
    indiceActivo = (index + itemsActuales.length) % itemsActuales.length;
    results.querySelectorAll(".producto-search-option").forEach((item, itemIndex) => {
      item.classList.toggle("active", itemIndex === indiceActivo);
      if (itemIndex === indiceActivo) item.scrollIntoView({ block: "nearest" });
    });
  }

  function pintarResultados(items) {
    itemsActuales = items;
    indiceActivo = items.length ? 0 : -1;
    const rect = search.getBoundingClientRect();
    results.style.left = `${rect.left}px`;
    results.style.top = `${rect.bottom + 4}px`;
    results.style.width = `${rect.width}px`;
    results.innerHTML = items.length
      ? items.map((producto, index) => {
          const sku = skuProducto(producto);
          const codigoBarras = codigoBarrasProducto(producto);
          const meta = [`Código: ${codigoProducto(producto)}`, sku ? `SKU: ${sku}` : "SKU: -", codigoBarras ? `CB: ${codigoBarras}` : "", `Stock: ${stockProducto(producto).toFixed(2)} ${unidadProducto(producto)}`].filter(Boolean).join(" | ");
          return `<button type="button" class="producto-search-option${index === 0 ? " active" : ""}" data-id="${escapeHtml(producto.id)}"><strong>${escapeHtml(textoProducto(producto))}</strong><span>${escapeHtml(meta)}</span><em>Precio: ${formatMoney(precioProducto(producto))}</em></button>`;
        }).join("")
      : '<div class="producto-search-empty">Sin coincidencias</div>';
    results.hidden = false;
  }

  async function mostrarResultados() {
    const items = await buscarRemoto(search.value);
    pintarResultados(items);
    return items;
  }

  async function agregarDesdeBusqueda() {
    if (!search.value.trim() && results.hidden) {
      await mostrarResultados();
      search.focus();
      return;
    }
    if (itemsActuales.length === 0) await mostrarResultados();
    const producto = itemsActuales[indiceActivo] || itemsActuales[0];
    if (!producto) return;
    agregarProducto(producto);
    search.value = "";
    cerrarResultados();
    search.focus();
  }

  body.querySelectorAll("tr").forEach(enlazarFila);
  renumerar();
  recalcular();

  table.closest("form")?.addEventListener("submit", function () {
    body.querySelectorAll("tr").forEach((row) => {
      const precio = row.querySelector(".precio-input");
      const descuento = row.querySelector(".descuento-input");
      const descuentoValue = Math.min(Math.max(Number(descuento?.value || 0), 0), 100);
      if (!precio || descuentoValue <= 0) return;
      precio.value = (Number(precio.value || 0) * (1 - descuentoValue / 100)).toFixed(2);
      descuento.value = "0";
    });
  });

  body.addEventListener("click", function (event) {
    const removeButton = event.target.closest(".remove-row");
    if (!removeButton) return;
    removeButton.closest("tr")?.remove();
    renumerar();
    recalcular();
    search.focus();
  });

  search.addEventListener("input", mostrarResultados);
  search.addEventListener("focus", mostrarResultados);
  search.addEventListener("keydown", function (event) {
    if (event.key === "ArrowDown") {
      event.preventDefault();
      if (results.hidden) mostrarResultados();
      else setActiveIndex(indiceActivo + 1);
      return;
    }
    if (event.key === "ArrowUp") {
      event.preventDefault();
      if (results.hidden) mostrarResultados();
      else setActiveIndex(indiceActivo - 1);
      return;
    }
    if (event.key === "Enter") {
      event.preventDefault();
      agregarDesdeBusqueda();
      return;
    }
    if (event.key === "Escape") {
      event.preventDefault();
      cerrarResultados();
    }
  });
  search.addEventListener("blur", function () {
    window.setTimeout(function () {
      if (!punteroEnResultados) cerrarResultados();
    }, 150);
  });
  toggle?.addEventListener("click", function () {
    search.focus();
    mostrarResultados();
  });
  add.addEventListener("click", async function () {
    if (!search.value.trim()) {
      search.focus();
      await mostrarResultados();
      return;
    }
    await agregarDesdeBusqueda();
  });
  results.addEventListener("pointerenter", function () {
    punteroEnResultados = true;
  });
  results.addEventListener("pointerleave", function () {
    punteroEnResultados = false;
    if (document.activeElement !== search) cerrarResultados();
  });
  results.addEventListener("mousemove", function (event) {
    const option = event.target.closest(".producto-search-option");
    if (!option) return;
    const options = Array.from(results.querySelectorAll(".producto-search-option"));
    setActiveIndex(options.indexOf(option));
  });
  results.addEventListener("mousedown", function (event) {
    const option = event.target.closest(".producto-search-option");
    if (!option) return;
    event.preventDefault();
    const producto = obtenerProducto(option.dataset.id);
    if (producto) agregarProducto(producto);
    search.value = "";
    cerrarResultados();
    search.focus();
  });
  window.addEventListener("scroll", cerrarResultadosPorScroll, true);
  window.addEventListener("resize", cerrarResultados);
};window.viveroStartSunatSync = function (url, empresaId) {
  const key = `viveroSunatSyncStarted:${empresaId || "default"}`;
  if (!url || sessionStorage.getItem(key) === "true") return;
  sessionStorage.setItem(key, "true");

  const start = function () {
    if (navigator.sendBeacon) {
      navigator.sendBeacon(url, new Blob([], { type: "application/x-www-form-urlencoded" }));
      return;
    }

    fetch(url, { method: "POST", keepalive: true }).catch(() => {});
  };

  if ("requestIdleCallback" in window) {
    window.requestIdleCallback(start, { timeout: 4000 });
    return;
  }

  window.setTimeout(start, 1500);
};

window.viveroCompraForm = function () {
  const table = document.querySelector("#compraDetalle");
  const body = table?.querySelector("tbody");
  const subtotalOutput = document.querySelector("#compraSubtotal");
  const igvOutput = document.querySelector("#compraIgv");
  const totalOutput = document.querySelector("#compraTotal");
  const pagadoOutput = document.querySelector("#compraPagado");
  const saldoOutput = document.querySelector("#compraSaldo");
  const estadoPagoOutput = document.querySelector("#compraEstadoPago");
  const formaPagoSelect = document.querySelector('[name="FormaPago"]');
  if (!table || !body) return;

  function formatMoney(value) {
    return `S/ ${Number(value || 0).toFixed(2)}`;
  }

  function productoOptions() {
    const select = body.querySelector(".producto-select");
    return select ? select.innerHTML : '<option value="">Seleccione producto</option>';
  }

  function recalcularTotales() {
    let subtotal = 0;
    let igv = 0;
    body.querySelectorAll("tr").forEach((row) => {
      const producto = row.querySelector(".producto-select");
      const cantidad = Number(row.querySelector(".cantidad-input")?.value || 0);
      const costo = Number(row.querySelector(".costo-input")?.value || 0);
      const totalLinea = cantidad * costo;
      const afectoIgv = producto?.selectedOptions?.[0]?.dataset.afectoIgv === "True"
        || producto?.selectedOptions?.[0]?.dataset.afectoIgv === "true";
      const importe = afectoIgv ? Math.round((totalLinea / 1.18) * 100) / 100 : totalLinea;
      const igvLinea = afectoIgv ? totalLinea - importe : 0;
      const total = row.querySelector(".line-total");
      if (total) total.value = totalLinea.toFixed(2);
      subtotal += importe;
      igv += igvLinea;
    });

    const totalCompra = subtotal + igv;
    const esContado = formaPagoSelect?.value === "CONTADO" || formaPagoSelect?.value === "1";
    if (subtotalOutput) subtotalOutput.textContent = formatMoney(subtotal);
    if (igvOutput) igvOutput.textContent = formatMoney(igv);
    if (totalOutput) totalOutput.textContent = formatMoney(totalCompra);
    if (pagadoOutput) pagadoOutput.textContent = formatMoney(esContado ? totalCompra : 0);
    if (saldoOutput) saldoOutput.textContent = formatMoney(esContado ? 0 : totalCompra);
    if (estadoPagoOutput) estadoPagoOutput.textContent = esContado ? "PAGADO" : "PENDIENTE";
  }

  function configurarFila(row) {
    row.querySelector(".producto-select")?.addEventListener("change", recalcularTotales);
    row.querySelector(".cantidad-input")?.addEventListener("input", recalcularTotales);
    row.querySelector(".costo-input")?.addEventListener("input", recalcularTotales);
  }

  body.querySelectorAll("tr").forEach(configurarFila);
  body.addEventListener("click", function (event) {
    const removeButton = event.target.closest(".remove-row");
    if (removeButton) {
      removeButton.closest("tr").remove();
      recalcularTotales();
      return;
    }

    const addButton = event.target.closest(".add-row");
    if (!addButton) return;

    const index = body.querySelectorAll("tr").length;
    const row = document.createElement("tr");
    row.innerHTML = `
      <td><select name="Detalles[${index}].ProductoId" class="form-select form-select-sm producto-select">${productoOptions()}</select><input name="Detalles[${index}].UnidadMedida" type="hidden" /></td>
      <td><input name="Detalles[${index}].Cantidad" class="form-control form-control-sm text-end cantidad-input" type="number" step="0.01" min="0.01" /></td>
      <td><input name="Detalles[${index}].CostoUnitario" class="form-control form-control-sm text-end costo-input" type="number" step="0.01" min="0" /></td>
      <td><input value="0.00" class="form-control form-control-sm text-end line-total" readonly /></td>
      <td class="text-center">
        <button type="button" class="icon-btn danger remove-row" title="Quitar fila" aria-label="Quitar fila">
          <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M18 6 6 18" /><path d="m6 6 12 12" /></svg>
        </button>
      </td>`;
    body.appendChild(row);
    configurarFila(row);
    recalcularTotales();
  });
  formaPagoSelect?.addEventListener("change", recalcularTotales);

  recalcularTotales();
};

window.viveroNotaPedidoForm = function (config) {
  window.viveroComprobanteForm({
    clientes: config?.clientes || [],
    productos: config?.productos || [],
    clientesUrl: config?.clientesUrl,
    productosUrl: config?.productosUrl
  });
};

window.viveroCobroForm = function () {
  const form = document.querySelector(".cobro-form");
  if (!form) return;

  const saldo = Number(form.dataset.saldo || 0);
  const monto = form.querySelector(".cobro-monto");
  form.addEventListener("submit", function (event) {
    const valor = Number(monto?.value || 0);
    if (valor <= 0 || valor > saldo) {
      event.preventDefault();
      alert(`El cobro debe ser mayor a cero y no superar S/ ${saldo.toFixed(2)}.`);
    }
  });
};

function viveroShell() {
  const shell = document.querySelector(".app-shell");
  const toggle = document.querySelector("#menuToggle");
  const groups = Array.from(document.querySelectorAll(".sidebar .nav-group"));
  if (!shell) return;

  if (localStorage.getItem("viveroSidebarCollapsed") === "true") {
    shell.classList.add("sidebar-collapsed");
  }

  toggle?.addEventListener("click", function () {
    shell.classList.toggle("sidebar-collapsed");
    localStorage.setItem(
      "viveroSidebarCollapsed",
      shell.classList.contains("sidebar-collapsed") ? "true" : "false"
    );
  });

  groups.forEach(function (group) {
    group.addEventListener("toggle", function () {
      if (!group.open) return;
      groups.forEach(function (other) {
        if (other !== group) {
          other.open = false;
        }
      });
    });
  });

  groups
    .filter(function (group) {
      return group.open;
    })
    .slice(1)
    .forEach(function (group) {
      group.open = false;
    });
}

document.addEventListener("DOMContentLoaded", viveroShell);

function viveroStandardizeGrids() {
  const icons = {
    view: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M2 12s3.5-6 10-6 10 6 10 6-3.5 6-10 6-10-6-10-6Z" /><path d="M12 9a3 3 0 1 1 0 6 3 3 0 0 1 0-6Z" /></svg>',
    edit: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 20h9" /><path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4Z" /></svg>',
    money: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 2v20" /><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7H14a3.5 3.5 0 0 1 0 7H6" /></svg>',
    cancel: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M18 6 6 18" /><path d="m6 6 12 12" /></svg>',
    reject: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M18 6 6 18" /><path d="M8 6h10" /><path d="M8 18h10" /></svg>',
    expire: '<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 6v6l4 2" /><path d="M21 12a9 9 0 1 1-9-9" /><path d="M21 3v6h-6" /></svg>'
  };

  function moveActionsColumn(table) {
    const headerRow = table.tHead?.rows[0];
    if (!headerRow) return;
    const headers = Array.from(headerRow.cells);
    const index = headers.findIndex((cell) => cell.classList.contains("actions-heading") || cell.textContent.trim().toLowerCase() === "acciones");
    if (index <= 0) return;

    headerRow.insertBefore(headers[index], headerRow.cells[0]);
    Array.from(table.tBodies).forEach((body) => {
      Array.from(body.rows).forEach((row) => {
        if (row.cells.length <= index) return;
        row.insertBefore(row.cells[index], row.cells[0]);
      });
    });
  }

  function normalizeIcon(button) {
    if (button.querySelector("svg")) return;
    const label = `${button.getAttribute("aria-label") || ""} ${button.getAttribute("title") || ""} ${button.textContent || ""}`.toLowerCase();
    let icon = null;
    if (label.includes("visualizar") || label.includes("ver")) icon = icons.view;
    if (label.includes("editar")) icon = icons.edit;
    if (label.includes("cobro") || label.includes("pago") || label.includes("$")) icon = icons.money;
    if (label.includes("anular") || label.trim() === "x") icon = icons.cancel;
    if (label.includes("rechazar") || label.trim() === "r") icon = icons.reject;
    if (label.includes("vencida") || label.includes("vencer") || label.trim() === "v") icon = icons.expire;
    if (!icon) return;
    button.innerHTML = icon;
  }

  document.querySelectorAll(".data-grid").forEach(moveActionsColumn);
  document.querySelectorAll(".data-grid .icon-btn").forEach(normalizeIcon);
}

document.addEventListener("DOMContentLoaded", viveroStandardizeGrids);

function viveroPermissionTree() {
  const tree = document.querySelector(".permission-tree");
  if (!tree) return;

  function syncCheck(parent, selector, checkSelector) {
    const check = parent.querySelector(checkSelector);
    const items = Array.from(parent.querySelectorAll(selector));
    if (!check || items.length === 0) return;

    const checked = items.filter((item) => item.checked).length;
    check.checked = checked === items.length;
    check.indeterminate = checked > 0 && checked < items.length;
  }

  function syncAll() {
    tree.querySelectorAll("[data-permission-form]").forEach((form) => {
      syncCheck(form, ".permission-action", "[data-permission-form-check]");
    });

    tree.querySelectorAll("[data-permission-module]").forEach((module) => {
      syncCheck(module, ".permission-action", "[data-permission-module-check]");
    });
  }

  tree.addEventListener("change", function (event) {
    const target = event.target;
    if (!(target instanceof HTMLInputElement)) return;

    if (target.matches("[data-permission-module-check]")) {
      target.closest("[data-permission-module]")?.querySelectorAll(".permission-action, [data-permission-form-check]")
        .forEach((item) => {
          item.checked = target.checked;
          item.indeterminate = false;
        });
    }

    if (target.matches("[data-permission-form-check]")) {
      target.closest("[data-permission-form]")?.querySelectorAll(".permission-action")
        .forEach((item) => {
          item.checked = target.checked;
        });
    }

    syncAll();
  });

  syncAll();
}

document.addEventListener("DOMContentLoaded", viveroPermissionTree);

window.viveroCompraDocumentoForm = function () {
  const tipoDocumento = document.querySelector("#tipoDocumentoCompra");
  const serie = document.querySelector("#serieCompra");
  const numero = document.querySelector("#numeroCompra");
  const fields = Array.from(document.querySelectorAll(".compra-document-number-field"));
  if (!tipoDocumento || !serie || !numero || fields.length === 0) return;

  const documentosSinSerieNumero = new Set(["4", "5", "6", "7", "RECIBO", "NOTA_VENTA", "PENDIENTE_COMPROBANTE", "SIN_DOCUMENTO"]);

  function normalizarTipoDocumento() {
    const value = (tipoDocumento.value || "").trim().toUpperCase();
    const text = (tipoDocumento.selectedOptions?.[0]?.textContent || "").trim().toUpperCase();
    return { value, text };
  }

  function sincronizarDocumento() {
    const tipo = normalizarTipoDocumento();
    const ocultar = documentosSinSerieNumero.has(tipo.value)
      || tipo.text.includes("RECIBO")
      || tipo.text.includes("NOTA VENTA")
      || tipo.text.includes("PENDIENTE")
      || tipo.text.includes("SIN DOCUMENTO");

    fields.forEach((field) => {
      field.hidden = ocultar;
      field.classList.toggle("d-none", ocultar);
      field.style.display = ocultar ? "none" : "";
    });

    serie.required = !ocultar;
    numero.required = !ocultar;
    serie.toggleAttribute("required", !ocultar);
    numero.toggleAttribute("required", !ocultar);

    if (ocultar) {
      serie.value = "";
      numero.value = "";
    }
  }

  tipoDocumento.addEventListener("change", sincronizarDocumento);
  tipoDocumento.addEventListener("input", sincronizarDocumento);
  sincronizarDocumento();
};

