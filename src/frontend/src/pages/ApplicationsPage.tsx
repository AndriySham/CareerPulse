import React from 'react';
import { Kanban } from 'lucide-react';

export const ApplicationsPage: React.FC = () => {
  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3 border-b border-border/40 pb-4">
        <div className="p-2 rounded-lg bg-primary/10 text-primary">
          <Kanban className="h-6 w-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Applications & Kanban</h1>
          <p className="text-sm text-muted-foreground">
            Track and manage your job applications lifecycle.
          </p>
        </div>
      </div>
      <div className="rounded-xl border border-border/40 bg-card p-8 text-center shadow-sm">
        <p className="text-muted-foreground">Application board will load here.</p>
      </div>
    </div>
  );
};

export default ApplicationsPage;
