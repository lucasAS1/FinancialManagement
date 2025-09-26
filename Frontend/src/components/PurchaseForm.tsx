import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box,
  Button,
  TextField,
  Typography,
  Paper,
  FormControlLabel,
  Checkbox,
} from '@mui/material';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { financialManagementService, Purchase } from '../services/api';

export default function PurchaseForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [purchase, setPurchase] = useState<Partial<Purchase>>({
    cost: 0,
    dueDate: new Date().toISOString(),
    paid: false,
  });

  useEffect(() => {
    if (id) {
      loadPurchase(id);
    }
  }, [id]);

  const loadPurchase = async (_rowKey: string) => {
    try {
      // TODO: Implement get purchase by id
      
      // const response = await financialManagementService.getPurchase(rowKey);
      // setPurchase(response.data);
    } catch (error) {
      console.error('Error loading purchase:', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (id) {
        // TODO: Implement update purchase
      } else {
        await financialManagementService.createPurchase(purchase as Purchase);
      }
      navigate('/purchases');
    } catch (error) {
      console.error('Error saving purchase:', error);
    }
  };

  return (
    <Box sx={{ maxWidth: 600, mx: 'auto' }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h5" gutterBottom>
          {id ? 'Edit Purchase' : 'Add Purchase'}
        </Typography>
        <form onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="Cost"
            type="number"
            value={purchase.cost || 0}
            onChange={(e) => setPurchase({ ...purchase, cost: parseFloat(e.target.value) })}
            margin="normal"
            required
            InputProps={{
              startAdornment: 'R$',
            }}
          />
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Due Date"
              value={purchase.dueDate ? new Date(purchase.dueDate) : null}
              onChange={(date: Date | null) => setPurchase({ ...purchase, dueDate: date?.toISOString() })}
              sx={{ width: '100%', mt: 2 }}
            />
          </LocalizationProvider>
          <FormControlLabel
            control={
              <Checkbox
                checked={purchase.paid || false}
                onChange={(e) => setPurchase({ ...purchase, paid: e.target.checked })}
              />
            }
            label="Paid"
            sx={{ mt: 2 }}
          />
          <Box sx={{ mt: 2, display: 'flex', gap: 2 }}>
            <Button
              type="submit"
              variant="contained"
              color="primary"
            >
              {id ? 'Update' : 'Create'}
            </Button>
            <Button
              variant="outlined"
              onClick={() => navigate('/purchases')}
            >
              Cancel
            </Button>
          </Box>
        </form>
      </Paper>
    </Box>
  );
} 