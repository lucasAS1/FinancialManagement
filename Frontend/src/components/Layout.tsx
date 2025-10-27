import { ReactNode } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  AppBar,
  Box,
  CssBaseline,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
} from '@mui/material';
import {
  Menu as MenuIcon,
  CreditCard as CreditCardIcon,
  ShoppingCart as ShoppingCartIcon,
  Receipt as ReceiptIcon,
  Upload as UploadIcon,
  AccountBalance as AccountBalanceIcon,
} from '@mui/icons-material';

const drawerWidth = 240;

interface LayoutProps {
  children: ReactNode;
}

const menuItems = [
  { text: 'Credit Cards', icon: <CreditCardIcon />, path: '/credit-cards' },
  { text: 'Credit Card Bills', icon: <AccountBalanceIcon />, path: '/credit-card-bills' },
  { text: 'Purchases', icon: <ShoppingCartIcon />, path: '/purchases' },
  { text: 'Monthly Bills', icon: <ReceiptIcon />, path: '/monthly-bills' },
  { text: 'Upload Bill', icon: <UploadIcon />, path: '/bill-upload' },
];

export default function Layout({ children }: LayoutProps) {
  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div">
            Financial Management
          </Typography>
        </Toolbar>
      </AppBar>
      <Drawer
        variant="permanent"
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: drawerWidth,
            boxSizing: 'border-box',
          },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto' }}>
          <List>
            {menuItems.map((item) => (
              <ListItem
                key={item.text}
                component={RouterLink}
                to={item.path}
                sx={{
                  '&:hover': {
                    backgroundColor: 'rgba(0, 0, 0, 0.04)',
                  },
                }}
              >
                <ListItemIcon>{item.icon}</ListItemIcon>
                <ListItemText primary={item.text} />
              </ListItem>
            ))}
          </List>
        </Box>
      </Drawer>
      <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
        <Toolbar />
        {children}
      </Box>
    </Box>
  );
} 