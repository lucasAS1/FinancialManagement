import { useState, useEffect } from 'react';
import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  Typography,
  Box,
  Chip,
  IconButton,
  Tooltip,
  Button,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import AddIcon from '@mui/icons-material/Add';
import VisibilityIcon from '@mui/icons-material/Visibility';
import { financialManagementService } from '../services/api';

interface CreditCard {
  id: string;
  name: string;
  bank: string;
  lastFourDigits: string;
  dueDay: number;
  limit: number;
  availableLimit: number;
}

export default function CreditCardList() {
  const [creditCards, setCreditCards] = useState<CreditCard[]>([]);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchCreditCards = async () => {
      try {
        setLoading(true);
        // Replace with your actual API call
        const response = await financialManagementService.getCreditCards();
        
        setCreditCards(() => response.data as CreditCard[]);
      } catch (error) {
        console.error('Error fetching credit cards:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchCreditCards();
  }, []);

  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleDelete = async (id: string) => {
    try {
      // Replace with your actual delete API call
      await financialManagementService.deleteCreditCard(id);
      setCreditCards(creditCards.filter(card => card.id !== id));
    } catch (error) {
      console.error('Error deleting credit card:', error);
    }
  };

  const handleEdit = (id: string) => {
    // Implement edit functionality
    console.log('Edit credit card with ID:', id);
  };

  const handleViewBills = (id: string) => {
    // Implement view bills functionality
    console.log('View bills for credit card with ID:', id);
  };

  // Format currency
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(amount);
  };

  const getLimitUtilizationColor = (limit: number, availableLimit: number) => {
    const utilizationPercentage = ((limit - availableLimit) / limit) * 100;
    if (utilizationPercentage < 50) return 'success';
    if (utilizationPercentage < 75) return 'warning';
    return 'error';
  };

  return (
    <Box sx={{ width: '100%' }}>
      <Paper sx={{ width: '100%', mb: 2 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', p: 2 }}>
          <Typography variant="h6" component="div" sx={{ fontWeight: 'bold' }}>
            Cartões de Crédito
          </Typography>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            size="small"
            onClick={() => console.log('Add new credit card')}
          >
            Novo Cartão
          </Button>
        </Box>
        <TableContainer>
          <Table sx={{ minWidth: 750 }} aria-label="credit card table">
            <TableHead>
              <TableRow>
                <TableCell>Nome</TableCell>
                <TableCell>Banco</TableCell>
                <TableCell>Final</TableCell>
                <TableCell>Vencimento</TableCell>
                {/* TODO: Remover colunas de limite e disponível conforme solicitado */}
                <TableCell align="right">Limite</TableCell>
                <TableCell align="right">Disponível</TableCell>
                <TableCell align="center">Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={7} align="center">
                    Carregando...
                  </TableCell>
                </TableRow>
              ) : creditCards.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} align="center">
                    Nenhum cartão de crédito encontrado
                  </TableCell>
                </TableRow>
              ) : (
                creditCards
                  .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                  .map((card) => (
                    <TableRow key={card.id} hover>
                      <TableCell component="th" scope="row">
                        {card.name}
                      </TableCell>
                      <TableCell>{card.bank}</TableCell>
                      <TableCell>**** {card.lastFourDigits}</TableCell>
                      <TableCell>Dia {card.dueDay}</TableCell>
                      <TableCell align="right">{formatCurrency(card.limit)}</TableCell>
                      <TableCell align="right">
                        <Chip
                          label={formatCurrency(card.availableLimit)}
                          size="small"
                          color={getLimitUtilizationColor(card.limit, card.availableLimit)}
                        />
                      </TableCell>
                      <TableCell align="center">
                        <Tooltip title="Ver Faturas">
                          <IconButton
                            size="small"
                            onClick={() => handleViewBills(card.id)}
                            color="primary"
                          >
                            <VisibilityIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Editar">
                          <IconButton
                            size="small"
                            onClick={() => handleEdit(card.id)}
                            color="primary"
                          >
                            <EditIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Excluir">
                          <IconButton
                            size="small"
                            onClick={() => handleDelete(card.id)}
                            color="error"
                          >
                            <DeleteIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      </TableCell>
                    </TableRow>
                  ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
        <TablePagination
          rowsPerPageOptions={[5, 10, 25]}
          component="div"
          count={creditCards.length}
          rowsPerPage={rowsPerPage}
          page={page}
          onPageChange={handleChangePage}
          onRowsPerPageChange={handleChangeRowsPerPage}
          labelRowsPerPage="Itens por página"
          labelDisplayedRows={({ from, to, count }) =>
            `${from}-${to} de ${count !== -1 ? count : `mais de ${to}`}`
          }
        />
      </Paper>
    </Box>
  );
}