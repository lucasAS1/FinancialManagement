import axios from 'axios';

const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:7243/api';

interface ETag {
  [key: string]: unknown;
}

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Add request interceptor for authentication if needed
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interfaces baseadas na documentação OpenAPI
export interface CreditCard {
  id: string; // Usando rowKey como id
  name: string;
  flag: string;
  bank: string;
  lastFourDigits: string;
  dueDay: number;
  limit: number;
  availableLimit: number;
  isDeleted: boolean;
  partitionKey?: string;
  rowKey?: string;
  timestamp?: string;
  eTag?: ETag;
}

export interface Purchase {
  billName: string;
  cost: number;
  dueDate: string;
  paid: boolean;
  timestamp?: string;
  eTag?: ETag;
  partitionKey?: string;
  rowKey?: string;
}

export interface MonthlyBill {
  billName: string;
  value: number;
  paid: boolean;
  rowKey?: string; // Adicionado para facilitar a identificação
}

export interface CreditCardBill {
  partitionKey: string;
  rowKey: string;
  dueDate: Date;
  paid: boolean;
  purchases: Purchase[];
  timestamp?: string;
  eTag?: ETag;
}

export enum CsvBillType {
  Itau = 0,
  Santander = 1,
  XP = 2,
  NuBank = 3,
  Other = 4
}

export const creditCardService = {
  getAll: () => apiClient.get<CreditCard[]>('/CreditCard'),
  getByName: (name: string) => apiClient.get<CreditCard>(`/CreditCard/${name}`),
  create: (name: string) => apiClient.post('/CreditCard', name),
  update: (creditCard: CreditCard) => apiClient.put('/CreditCard', creditCard),
  delete: (rowKey: string) => apiClient.delete(`/CreditCard/${rowKey}`),
  getBills: () => apiClient.get<CreditCardBill[]>('/CreditCard/bills'),
  getBillsByCard: (cardName: string) => apiClient.get<CreditCardBill[]>(`/CreditCard/${cardName}/bills`),
};

export const financialManagementService = {
  // Credit Cards (CreditCardController)
  getCreditCards: () => apiClient.get<CreditCard[]>('/CreditCard'),
  getCreditCardByName: (name: string) => apiClient.get<CreditCard>(`/CreditCard/${name}`),
  createCreditCard: (name: string) => apiClient.post('/CreditCard', JSON.stringify(name)),
  updateCreditCard: (creditCard: CreditCard) => apiClient.put('/CreditCard', creditCard),
  deleteCreditCard: (rowKey: string) => apiClient.delete(`/CreditCard/${rowKey}`),

  // Financial Management (FinancialManagementController)
  getMonthlyBills: (date: string) => apiClient.get<MonthlyBill[]>(`/FinancialManagement/${date}`),

  // Purchases
  createPurchase: (purchase: Purchase) => apiClient.post('/FinancialManagement/purchase', purchase),
  createCreditCardPurchase: (creditCard: string, purchase: Purchase) =>
      apiClient.post(`/FinancialManagement/credit-card/${creditCard}/purchase`, purchase),

  // Credit Card Bill Upload
  uploadBill: (bank: CsvBillType, month: string, cardName: string | null, file: File) => {
    const formData = new FormData();
    formData.append('file', file);

    // Usando query params conforme a documentação OpenAPI
    const queryParams = new URLSearchParams();
    queryParams.append('bank', bank.toString());
    queryParams.append('month', month);
    if (cardName) queryParams.append('cardName', cardName);

    return apiClient.post(`/FinancialManagement/bill?${queryParams.toString()}`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
  },

  // Métodos adicionais que não estão na documentação mas são necessários para a UI
  getPurchases: () => apiClient.get('/FinancialManagement/purchases'),
  deletePurchase: (id: string) => apiClient.delete(`/FinancialManagement/purchase/${id}`),
  deleteBill: (id: string) => apiClient.delete(`/FinancialManagement/bill/${id}`)
};