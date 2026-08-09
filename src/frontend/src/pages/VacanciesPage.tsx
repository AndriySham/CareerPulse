import React from 'react';
import { Briefcase } from 'lucide-react';

export const VacanciesPage: React.FC = () => {
  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3 border-b border-border/40 pb-4">
        <div className="p-2 rounded-lg bg-primary/10 text-primary">
          <Briefcase className="h-6 w-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Vacancies</h1>
          <p className="text-sm text-muted-foreground">
            View and track targeted job opportunities and vacancy requirements.
          </p>
        </div>
      </div>
      <div className="rounded-xl border border-border/40 bg-card p-8 text-center shadow-sm">
        <p className="text-muted-foreground">Vacancy list will load here.</p>
      </div>
    </div>
  );
};

export default VacanciesPage;
