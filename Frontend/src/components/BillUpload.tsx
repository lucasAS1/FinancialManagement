import { useState } from 'react';
import {
  Box,
  Button,
  TextField,
  Typography,
  Paper,
  MenuItem,
} from '@mui/material';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { financialManagementService, CsvBillType } from '../services/api';

const bankOptions = [
  { value: CsvBillType.Itau, label: 'Itaú' },
  { value: CsvBillType.Santander, label: 'Santander' },
  { value: CsvBillType.XP, label: 'XP' },
  { value: CsvBillType.NuBank, label: 'NuBank' },
  { value: CsvBillType.Other, label: 'Outro' }
];

export default function BillUpload() {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [bank, setBank] = useState<number>(0);
  const [month, setMonth] = useState<Date>(new Date());
  const [cardName, setCardName] = useState<string>('');

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files[0]) {
      setSelectedFile(event.target.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedFile) return;

    try {
      await financialManagementService.uploadBill(
        bank,
        month.toISOString(),
        cardName,
        selectedFile
      );
      // Reset form
      setSelectedFile(null);
      setBank(0);
      setMonth(new Date());
      setCardName('');
    } catch (error) {
      console.error('Error uploading bill:', error);
    }
  };

  return (
    <Box sx={{ maxWidth: 600, mx: 'auto' }}>
      <Paper sx={{ p: 3 }}>
        <Typography variant="h5" gutterBottom>
          Upload Bill
        </Typography>
        <form onSubmit={handleSubmit}>
          <TextField
            fullWidth
            select
            label="Bank"
            value={bank}
            onChange={(e) => setBank(Number(e.target.value))}
            margin="normal"
            required
          >
            {bankOptions.map((option) => (
              <MenuItem key={option.value} value={option.value}>
                {option.label}
              </MenuItem>
            ))}
          </TextField>
          <LocalizationProvider dateAdapter={AdapterDateFns}>
            <DatePicker
              label="Month"
              value={month}
              onChange={(date: Date | null) => date && setMonth(date)}
              sx={{ width: '100%', mt: 2 }}
              views={['year', 'month']}
            />
          </LocalizationProvider>
          <TextField
            fullWidth
            label="Card Name"
            value={cardName}
            onChange={(e) => setCardName(e.target.value)}
            margin="normal"
          />
          <Button
            variant="contained"
            component="label"
            sx={{ mt: 2 }}
          >
            Select File
            <input
              type="file"
              hidden
              onChange={handleFileChange}
            />
          </Button>
          {selectedFile && (
            <Typography sx={{ mt: 1 }}>
              Selected file: {selectedFile.name}
            </Typography>
          )}
          <Box sx={{ mt: 2 }}>
            <Button
              type="submit"
              variant="contained"
              color="primary"
              disabled={!selectedFile}
            >
              Upload
            </Button>
          </Box>
        </form>
      </Paper>
    </Box>
  );
} 