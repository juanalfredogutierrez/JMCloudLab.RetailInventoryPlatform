export const API = {
  gateway: 'http://localhost:5000',

  auth: {
    login: '/auth/login'
  },

  productos: {
    list: '/productos',
    create: '/productos'
  },

  transaccion: {
    compras: '/transacciones/compra',
    ventas: '/transacciones/venta'
  },

  

  inventario: {
    stock: '/inventario/stock'
  }
};