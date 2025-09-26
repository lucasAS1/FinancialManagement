# Financial Management Frontend

This is the frontend application for the Financial Management system, built with React and Material-UI.

## Features

- Credit Card Management
  - List all credit cards
  - Add new credit cards
  - Edit existing credit cards
  - Delete credit cards

- Purchase Management
  - List all purchases
  - Add new purchases
  - Edit existing purchases
  - Delete purchases

- Monthly Bills
  - View monthly bills
  - Filter bills by month
  - View total amounts (paid and pending)

- Bill Upload
  - Upload bill files from different banks
  - Select month and card name
  - View upload status

## Prerequisites

- Node.js (v18 or higher)
- npm (v10 or higher)

## Installation

1. Clone the repository
2. Navigate to the Frontend directory
3. Install dependencies:

```bash
npm install
```

## Running the Application

1. Start the development server:

```bash
npm run dev
```

2. Open your browser and navigate to `http://localhost:5173`

## Building for Production

To create a production build:

```bash
npm run build
```

The build files will be created in the `dist` directory.

## API Configuration

The application expects the backend API to be running at `http://localhost:5000`. If your backend is running at a different URL, update the `baseURL` in `src/services/api.ts`.

## Technologies Used

- React
- TypeScript
- Material-UI
- React Router
- Axios
- date-fns
