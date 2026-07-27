import { TopbarInfo } from "./TopbarInfo.model";

export const TOPBAR_ROUTES: Record<string, TopbarInfo> = {

    '/dashboard': {
        section: 'Dashboard',
        title: 'Resumen general'
    },

    '/productos': {
        section: 'Productos',
        title: 'Gestión de productos'
    },

    '/compras': {
        section: 'Compras',
        title: 'Registrar compra'

    },

    '/ventas': {
        section: 'Ventas',
        title: 'Registrar venta'
    },

    '/kardex': {
        section: 'Inventario',
        title: 'Kardex de movimientos'
    }

};