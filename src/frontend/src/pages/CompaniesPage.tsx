import React from 'react';
import { Building2 } from 'lucide-react';

export const CompaniesPage: React.FC = () => {
  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3 border-b border-border/40 pb-4">
        <div className="p-2 rounded-lg bg-primary/10 text-primary">
          <Building2 className="h-6 w-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Companies</h1>
          <p className="text-sm text-muted-foreground">
            Manage target employers and historical company contacts.
          </p>
        </div>
      </div>
      <div className="rounded-xl border border-border/40 bg-card p-8 text-center shadow-sm">
        <p className="text-muted-foreground">Company directory will load here.</p>
      </div>
    </div>
  );
};

export default CompaniesPage;
