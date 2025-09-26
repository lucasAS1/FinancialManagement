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
  Select,
  MenuItem,
  FormControl,
  InputLabel
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import { financialManagementService } from '../services/api';

interface Bill {
  billName: string;
  value: number;
  paid: boolean;
}

export default function MonthlyBills() {
  const [bills, setBills] = useState<Bill[]>([]);
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState('all');

  const currentDate = new Date();
  const months = Array.from({ length: 12 }, (_, i) => {
    const date = new Date(currentDate.getFullYear(), currentDate.getMonth() - i, 1);
    return {
      value: format(date, 'yyyy-MM'),
      label: format(date, 'MMMM yyyy', { locale: ptBR })
    };
  });

  useEffect(() => {
    const fetchBills = async () => {
      try {
        setLoading(true);
        // Replace with your actual API call
        const response = await financialManagementService.getMonthlyBills(filter);
        setBills(() => response.data as Bill[]);
      } catch (error) {
        console.error('Error fetching bills:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchBills();
  }, [filter]);

  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleDelete = async (billName: string) => {
    try {
      if (!billName) return;
      // Replace with your actual delete API call
      await financialManagementService.deleteBill(billName);
      setBills(bills.filter(bill => bill.billName !== billName));
    } catch (error) {
      console.error('Error deleting bill:', error);
    }
  };

  const handleEdit = (billName: string) => {
    if (!billName) return;
    // TODO: Implementar funcionalidade de edição de contas mensais
    // 1. Criar formulário de edição
    // 2. Carregar dados da conta selecionada
    // 3. Implementar chamada à API para atualizar a conta
    console.log('Edit bill with billName:', billName);
  };

  // Format currency
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(amount);
  };

  const getStatusChip = (status: string) => {
    switch (status) {
      case 'paid':
        return <Chip label="Pago" size="small" color="success" />;
      case 'pending':
        return <Chip label="Pendente" size="small" color="warning" />;
      case 'overdue':
        return <Chip label="Atrasado" size="small" color="error" />;
      default:
        return <Chip label={status} size="small" />;
    }
  };

  const getFilteredBills = () => {
      return bills;
  };

  const filteredBills = getFilteredBills();
  console.log(filteredBills);
  return (
    <Box sx={{ width: '100%' }}>
      <Paper sx={{ width: '100%', mb: 2 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', p: 2 }}>
          <Typography variant="h6" component="div" sx={{ fontWeight: 'bold' }}>
            Faturas Mensais
          </Typography>
          <FormControl sx={{ minWidth: 200 }}>
            <InputLabel id="month-filter-label">Período</InputLabel>
            <Select
              labelId="month-filter-label"
              value={filter}
              label="Período"
              onChange={(e) => setFilter(e.target.value)}
              size="small"
            >
              <MenuItem value="all">Todos os meses</MenuItem>
              {months.map((month) => (
                <MenuItem key={month.value} value={month.value}>
                  {month.label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Box>
        <TableContainer>
          <Table sx={{ minWidth: 750 }} aria-label="monthly bills table">
            <TableHead>
              <TableRow>
                <TableCell>Nome da Conta</TableCell>
                <TableCell align="right">Valor</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="center">Ações</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? (
                <TableRow>
                  <TableCell colSpan={8} align="center">
                    Carregando...
                  </TableCell>
                </TableRow>
              ) : filteredBills.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={8} align="center">
                    Nenhuma fatura encontrada
                  </TableCell>
                </TableRow>
              ) : (
                filteredBills
                  .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                  .map((bill, index) => (
                    <TableRow key={bill.billName || index} hover>
                      <TableCell component="th" scope="row">
                        {bill.billName}
                      </TableCell>
                      <TableCell align="right">
                        {formatCurrency(bill.value)}
                      </TableCell>
                      <TableCell>
                        {getStatusChip(bill.paid ? 'paid' : 'pending')}
                      </TableCell>
                      <TableCell align="center">
                        <Tooltip title="Editar">
                          <IconButton
                            size="small"
                            onClick={() => handleEdit(bill.billName || '')}
                            color="primary"
                          >
                            <EditIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Excluir">
                          <IconButton
                            size="small"
                            onClick={() => handleDelete(bill.billName || '')}
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
          count={filteredBills.length}
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