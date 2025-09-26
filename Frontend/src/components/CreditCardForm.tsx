import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box,
  Button,
  TextField,
  Typography,
  Paper,
} from '@mui/material';
import { creditCardService, CreditCard } from '../services/api';

export default function CreditCardForm() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [creditCard, setCreditCard] = useState<Partial<CreditCard>>({
    name: '',
    flag: '',
  });

  useEffect(() => {
    if (id) {
      loadCreditCard(id);
    }
  }, [id]);

  const loadCreditCard = async (rowKey: string) => {
    try {
      const response = await creditCardService.getByName(rowKey);
      setCreditCard(response.data);
    } catch (error) {
      console.error('Error loading credit card:', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (id) {
        await creditCardService.update(creditCard as CreditCard);
      } else {
        await creditCardService.create(creditCard.name || '');
      }
      navigate('/credit-cards');
    } catch (error) {
      console.error('Error saving credit card:', error);
    }
  };

  return (
    <Box sx={{ maxWidth: 600, mx: 'auto' }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h5" gutterBottom>
          {id ? 'Edit Credit Card' : 'Add Credit Card'}
        </Typography>
        <form onSubmit={handleSubmit}>
          <TextField
            fullWidth
            label="Name"
            value={creditCard.name || ''}
            onChange={(e) => setCreditCard({ ...creditCard, name: e.target.value })}
            margin="normal"
            required
          />
          <TextField
            fullWidth
            label="Flag"
            value={creditCard.flag || ''}
            onChange={(e) => setCreditCard({ ...creditCard, flag: e.target.value })}
            margin="normal"
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
              onClick={() => navigate('/credit-cards')}
            >
              Cancel
            </Button>
          </Box>
        </form>
      </Paper>
    </Box>
  );
} 