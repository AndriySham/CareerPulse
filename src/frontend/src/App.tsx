import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import MainLayout from './components/layout/MainLayout';
import ApplicationsPage from './pages/ApplicationsPage';
import CompaniesPage from './pages/CompaniesPage';
import VacanciesPage from './pages/VacanciesPage';
import ResumesPage from './pages/ResumesPage';
import SkillsPage from './pages/SkillsPage';

export const App: React.FC = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<Navigate to="/applications" replace />} />
          <Route path="applications" element={<ApplicationsPage />} />
          <Route path="companies" element={<CompaniesPage />} />
          <Route path="vacancies" element={<VacanciesPage />} />
          <Route path="resumes" element={<ResumesPage />} />
          <Route path="skills" element={<SkillsPage />} />
          <Route path="*" element={<Navigate to="/applications" replace />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default App;
