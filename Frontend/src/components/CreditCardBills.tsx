import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import {
  Box,
  Typography,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Chip,
  CircularProgress,
  Button,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { creditCardService, CreditCardBill } from '../services/api';

export default function CreditCardBills() {
  const { cardName } = useParams<{ cardName: string }>();
  const [bills, setBills] = useState<CreditCardBill[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadBills();
  }, [cardName]);

  const loadBills = async () => {
    try {
      if (cardName) {
        const response = await creditCardService.getBillsByCard(decodeURIComponent(cardName));
        setBills(response.data);
      } else {
        const response = await creditCardService.getBills();
        setBills(response.data);
      }
    } catch (error) {
      console.error('Error loading credit card bills:', error);
    } finally {
      setLoading(false);
    }
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('pt-BR');
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ maxWidth: '100%', mx: 'auto' }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
        {cardName && (
          <Button
            startIcon={<ArrowBackIcon />}
            onClick={() => window.history.back()}
            sx={{ mr: 2 }}
          >
            Back
          </Button>
        )}
        <Typography variant="h4">
          {cardName ? `Bills for ${decodeURIComponent(cardName)}` : 'Credit Card Bills'}
        </Typography>
      </Box>

      {bills.length === 0 ? (
        <Paper sx={{ p: 3, textAlign: 'center' }}>
          <Typography variant="body1" color="text.secondary">
            {cardName
              ? `No bills found for ${decodeURIComponent(cardName)}.`
              : 'No credit card bills found.'
            }
          </Typography>
        </Paper>
      ) : (
        bills.map((bill) => (
          <Accordion key={`${bill.partitionKey}-${bill.rowKey}`} sx={{ mb: 2 }}>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
              <Box sx={{ display: 'flex', alignItems: 'center', width: '100%', gap: 2 }}>
                <Typography variant="h6">{bill.partitionKey}</Typography>
                <Typography variant="body2" color="text.secondary">
                  {bill.rowKey}
                </Typography>
                <Chip
                  label={bill.paid ? 'Paid' : 'Pending'}
                  color={bill.paid ? 'success' : 'warning'}
                  size="small"
                />
                <Typography variant="body2" sx={{ ml: 'auto' }}>
                  Due: {formatDate(bill.dueDate.toString())}
                </Typography>
              </Box>
            </AccordionSummary>
            <AccordionDetails>
              <TableContainer component={Paper} variant="outlined">
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>Description</TableCell>
                      <TableCell align="right">Cost</TableCell>
                      <TableCell align="center">Due Date</TableCell>
                      <TableCell align="center">Status</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {bill.purchases && bill.purchases.length > 0 ? (
                      bill.purchases.map((purchase, index) => (
                        <TableRow key={index}>
                          <TableCell>{purchase.rowKey || 'Purchase'}</TableCell>
                          <TableCell align="right">{formatCurrency(purchase.cost)}</TableCell>
                          <TableCell align="center">{formatDate(purchase.dueDate.toString())}</TableCell>
                          <TableCell align="center">
                            <Chip
                              label={purchase.paid ? 'Paid' : 'Pending'}
                              color={purchase.paid ? 'success' : 'warning'}
                              size="small"
                            />
                          </TableCell>
                        </TableRow>
                      ))
                    ) : (
                      <TableRow>
                        <TableCell colSpan={4} align="center" sx={{ py: 3 }}>
                          <Typography variant="body2" color="text.secondary">
                            No purchases in this bill
                          </Typography>
                        </TableCell>
                      </TableRow>
                    )}
                  </TableBody>
                </Table>
              </TableContainer>
            </AccordionDetails>
          </Accordion>
        ))
      )}
    </Box>
  );
}