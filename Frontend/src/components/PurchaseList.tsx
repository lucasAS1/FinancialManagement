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
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import { format } from 'date-fns';
import {financialManagementService} from '../services/api';

interface Purchase {
  id: string;
  description: string;
  amount: number;
  date: string;
  category: string;
  paymentMethod: string;
}

export default function PurchaseList() {
  const [purchases, setPurchases] = useState<Purchase[]>([]);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPurchases = async () => {
      try {
        setLoading(true);
        // Replace with your actual API call
        const response = await financialManagementService.getPurchases();
        setPurchases(response.data);
      } catch (error) {
        console.error('Error fetching purchases:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchPurchases();
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
      await financialManagementService.deletePurchase(id);
      setPurchases(purchases.filter(purchase => purchase.id !== id));
    } catch (error) {
      console.error('Error deleting purchase:', error);
    }
  };

  const handleEdit = (id: string) => {
    // Implement edit functionality
    console.log('Edit purchase with ID:', id);
  };

  // Format currency
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(amount);
  };

  return (
    <Box sx={{ width: '100%' }}>
      <Paper sx={{ width: '100%', mb: 2 }}>
        <TableContainer>
          <Typography 
            variant="h6" 
            component="div" 
            sx={{ p: 2, fontWeight: 'bold' }}
          >
            Lista de Compras
          </Typography>
          <Table sx={{ minWidth: 750 }} aria-label="purchase table">
            <TableHead>
              <TableRow>
                <TableCell>Descrição</TableCell>
                <TableCell align="right">Valor</TableCell>
                <TableCell>Data</TableCell>
                <TableCell>Categoria</TableCell>
                <TableCell>Método de Pagamento</TableCell>
                <TableCell align="center">Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={6} align="center">
                    Carregando...
                  </TableCell>
                </TableRow>
              ) : purchases.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={6} align="center">
                    Nenhuma compra encontrada
                  </TableCell>
                </TableRow>
              ) : (
                purchases
                  .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                  .map((purchase) => (
                    <TableRow key={purchase.id} hover>
                      <TableCell component="th" scope="row">
                        {purchase.description}
                      </TableCell>
                      <TableCell align="right">
                        {formatCurrency(purchase.amount)}
                      </TableCell>
                      <TableCell>
                        {format(new Date(purchase.date), 'dd/MM/yyyy')}
                      </TableCell>
                      <TableCell>
                        <Chip 
                          label={purchase.category} 
                          size="small" 
                          color="primary" 
                          variant="outlined" 
                        />
                      </TableCell>
                      <TableCell>{purchase.paymentMethod}</TableCell>
                      <TableCell align="center">
                        <Tooltip title="Editar">
                          <IconButton 
                            size="small" 
                            onClick={() => handleEdit(purchase.id)}
                            color="primary"
                          >
                            <EditIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Excluir">
                          <IconButton 
                            size="small" 
                            onClick={() => handleDelete(purchase.id)}
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
          count={purchases.length}
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