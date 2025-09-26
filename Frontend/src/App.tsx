import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import Layout from './components/Layout.tsx';
import CreditCardList from './components/CreditCardList.tsx';
import CreditCardForm from './components/CreditCardForm.tsx';
import PurchaseList from './components/PurchaseList.tsx';
import PurchaseForm from './components/PurchaseForm.tsx';
import MonthlyBills from './components/MonthlyBills.tsx';
import BillUpload from './components/BillUpload.tsx';

const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Router>
        <Layout>
          <Routes>
            <Route path="/" element={<CreditCardList />} />
            <Route path="/credit-cards" element={<CreditCardList />} />
            <Route path="/credit-cards/new" element={<CreditCardForm />} />
            <Route path="/credit-cards/:id/edit" element={<CreditCardForm />} />
            <Route path="/purchases/:date" element={<PurchaseList date={new Date()} />} />
            <Route path="/purchases/new" element={<PurchaseForm />} />
            <Route path="/monthly-bills" element={<MonthlyBills />} />
            <Route path="/bill-upload" element={<BillUpload />} />
          </Routes>
        </Layout>
      </Router>
    </ThemeProvider>
  );
}

export default App;
