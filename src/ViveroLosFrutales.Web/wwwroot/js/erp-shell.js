(function () {
  "use strict";

  function updatePrimaryActionLabel() {
    const path = window.location.pathname.replace(/\/$/, "").toLocaleLowerCase("es");
    const actions = [
      { page: "/cotizaciones", href: "/Cotizaciones/Create", label: "Nueva cotización" },
      { page: "/notaspedido", href: "/NotasPedido/Nuevo", label: "Nueva nota de pedido" },
      { page: "/comprobantes", href: "/Comprobantes/Create", label: "Nuevo comprobante" },
      { page: "/ventas", href: "/Comprobantes/Create", label: "Nuevo comprobante" },
      { page: "/notascredito", href: "/NotasCredito/Create", label: "Nueva nota de crédito" }
    ];
    const current = actions.find(function (item) { return path.endsWith(item.page); });
    if (!current) return;
    const link = Array.from(document.querySelectorAll(".toolbar a")).find(function (item) {
      return new URL(item.href, window.location.origin).pathname.endsWith(current.href);
    });
    if (!link) return;
    link.textContent = current.label;
    link.setAttribute("aria-label", current.label);
  }

  function initializeShell() {
    const shell = document.querySelector(".app-shell");
    const sidebar = document.querySelector(".app-sidebar");
    if (!shell || !sidebar) return;

    const desktopToggle = document.querySelector("#desktopSidebarToggle");
    const mobileToggle = document.querySelector("#mobileSidebarToggle");
    const closeButton = document.querySelector("#mobileSidebarClose");
    const overlay = document.querySelector(".sidebar-overlay");
    const search = document.querySelector("#systemMenuSearch");
    const groups = Array.from(sidebar.querySelectorAll(".sidebar-section"));
    const mobileQuery = window.matchMedia("(max-width: 900px)");

    function isMobile() { return mobileQuery.matches; }

    function updateDesktopState(collapsed) {
      const label = collapsed ? "Expandir menú" : "Contraer menú";
      shell.classList.toggle("sidebar-collapsed", collapsed);
      desktopToggle?.setAttribute("aria-expanded", String(!collapsed));
      desktopToggle?.setAttribute("aria-label", label);
      desktopToggle?.setAttribute("title", label);
      localStorage.setItem("viveroSidebarCollapsed", String(collapsed));
    }

    function openMobileMenu() {
      shell.classList.add("mobile-menu-open");
      document.body.classList.add("menu-open");
      mobileToggle?.setAttribute("aria-expanded", "true");
      window.setTimeout(function () { closeButton?.focus(); }, 150);
    }

    function closeMobileMenu() {
      shell.classList.remove("mobile-menu-open");
      document.body.classList.remove("menu-open");
      mobileToggle?.setAttribute("aria-expanded", "false");
    }

    if (!isMobile()) updateDesktopState(localStorage.getItem("viveroSidebarCollapsed") === "true");

    desktopToggle?.addEventListener("click", function () {
      updateDesktopState(!shell.classList.contains("sidebar-collapsed"));
    });
    mobileToggle?.addEventListener("click", openMobileMenu);
    closeButton?.addEventListener("click", closeMobileMenu);
    overlay?.addEventListener("click", closeMobileMenu);

    sidebar.querySelectorAll("a.sidebar-item").forEach(function (link) {
      link.addEventListener("click", function () { if (isMobile()) closeMobileMenu(); });
    });

    groups.forEach(function (group) {
      group.addEventListener("toggle", function () {
        if (!group.open) return;
        groups.forEach(function (other) { if (other !== group) other.open = false; });
      });
      group.querySelector(":scope > summary")?.addEventListener("click", function () {
        if (!isMobile() && shell.classList.contains("sidebar-collapsed")) updateDesktopState(false);
      });
    });

    search?.addEventListener("input", function () {
      const query = search.value.trim().toLocaleLowerCase("es");
      groups.forEach(function (group) {
        const links = Array.from(group.querySelectorAll(".sidebar-submenu .sidebar-item"));
        let matches = false;
        links.forEach(function (link) {
          const visible = !query || link.textContent.toLocaleLowerCase("es").includes(query);
          link.hidden = !visible;
          matches = matches || visible;
        });
        group.hidden = Boolean(query) && !matches && !group.textContent.toLocaleLowerCase("es").includes(query);
        if (query && matches) group.open = true;
      });
      sidebar.querySelectorAll(":scope .sidebar-nav > .sidebar-item").forEach(function (link) {
        link.hidden = Boolean(query) && !link.textContent.toLocaleLowerCase("es").includes(query);
      });
    });

    document.addEventListener("keydown", function (event) {
      if (event.key === "Escape" && shell.classList.contains("mobile-menu-open")) closeMobileMenu();
    });

    mobileQuery.addEventListener("change", function () {
      closeMobileMenu();
      if (!isMobile()) updateDesktopState(localStorage.getItem("viveroSidebarCollapsed") === "true");
    });
  }

  document.addEventListener("DOMContentLoaded", function () {
    initializeShell();
    updatePrimaryActionLabel();
  });
})();
