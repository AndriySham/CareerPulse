import React, { useState } from 'react';
import type { ApplicationDto, ApplicationStatus } from '@/types';
import ApplicationCard from './ApplicationCard';
import { STATUS_CONFIG } from './ApplicationStatusBadge';
import { Layers } from 'lucide-react';

interface KanbanColumnProps {
  status: ApplicationStatus;
  applications: ApplicationDto[];
  onViewApplication: (app: ApplicationDto) => void;
  onChangeStatus: (app: ApplicationDto, newStatus: ApplicationStatus) => void;
  onDropApplication: (appId: string, targetStatus: ApplicationStatus) => void;
}

export const KanbanColumn: React.FC<KanbanColumnProps> = ({
  status,
  applications,
  onViewApplication,
  onChangeStatus,
  onDropApplication,
}) => {
  const [isDragOver, setIsDragOver] = useState(false);
  const [isAllowedDrop, setIsAllowedDrop] = useState(true);

  const statusInfo = STATUS_CONFIG[status];
  const Icon = statusInfo?.icon || Layers;

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';

    if (!isDragOver) {
      setIsDragOver(true);
      try {
        const appJson = e.dataTransfer.getData('application/json');
        if (appJson) {
          const app: ApplicationDto = JSON.parse(appJson);
          setIsAllowedDrop(
            app.status === status || Boolean(app.allowedTransitions?.includes(status))
          );
        }
      } catch {
        setIsAllowedDrop(true);
      }
    }
  };

  const handleDragLeave = () => {
    setIsDragOver(false);
    setIsAllowedDrop(true);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
    setIsAllowedDrop(true);

    const appId = e.dataTransfer.getData('text/plain');
    if (appId) {
      onDropApplication(appId, status);
    }
  };

  return (
    <div
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
      className={`flex flex-col w-72 shrink-0 rounded-xl border bg-card/60 p-3 transition-all duration-200 ${
        isDragOver
          ? isAllowedDrop
            ? 'border-primary ring-2 ring-primary/30 bg-primary/5'
            : 'border-destructive ring-2 ring-destructive/30 bg-destructive/5'
          : 'border-border/60 hover:border-border'
      }`}
    >
      {/* Column Header */}
      <div className="flex items-center justify-between gap-2 pb-3 mb-2 border-b border-border/40 px-1">
        <div className="flex items-center gap-2">
          <div className={`p-1.5 rounded-lg ${statusInfo?.badgeClass}`}>
            <Icon className="h-4 w-4" />
          </div>
          <h3 className="font-bold text-sm text-foreground tracking-tight">
            {statusInfo?.label ?? status}
          </h3>
        </div>
        <span className="flex h-5 w-5 items-center justify-center rounded-full bg-secondary text-[11px] font-bold text-secondary-foreground">
          {applications.length}
        </span>
      </div>

      {/* Applications List */}
      <div className="flex-1 space-y-3 overflow-y-auto min-h-[450px] max-h-[calc(100vh-280px)] pr-1 custom-scrollbar">
        {applications.length === 0 ? (
          <div className="h-32 flex flex-col items-center justify-center rounded-lg border border-dashed border-border/60 text-center p-3">
            <span className="text-xs text-muted-foreground/50 italic">No applications</span>
          </div>
        ) : (
          applications.map((app) => (
            <ApplicationCard
              key={app.id}
              application={app}
              onView={onViewApplication}
              onChangeStatus={onChangeStatus}
            />
          ))
        )}
      </div>
    </div>
  );
};

export default KanbanColumn;
