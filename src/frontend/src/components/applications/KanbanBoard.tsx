import React, { useState, useMemo } from 'react';
import type { ApplicationDto, ApplicationStatus } from '@/types';
import KanbanColumn from './KanbanColumn';
import { STATUS_CONFIG } from './ApplicationStatusBadge';

interface KanbanBoardProps {
  applications: ApplicationDto[];
  onViewApplication: (app: ApplicationDto) => void;
  onChangeStatus: (app: ApplicationDto, newStatus: ApplicationStatus) => void;
}

const KANBAN_STATUSES: ApplicationStatus[] = [
  'Draft',
  'Applied',
  'Viewed',
  'HRInterview',
  'TechnicalInterview',
  'Offer',
  'Rejected',
  'NoResponse',
];

export const KanbanBoard: React.FC<KanbanBoardProps> = ({
  applications,
  onViewApplication,
  onChangeStatus,
}) => {
  const [activeMobileStatus, setActiveMobileStatus] = useState<ApplicationStatus | 'All'>('All');

  // Group applications by status
  const applicationsByStatus = useMemo(() => {
    const map: Record<ApplicationStatus, ApplicationDto[]> = {
      Draft: [],
      Applied: [],
      Viewed: [],
      HRInterview: [],
      TechnicalInterview: [],
      Offer: [],
      Rejected: [],
      NoResponse: [],
    };

    applications.forEach((app) => {
      if (map[app.status]) {
        map[app.status].push(app);
      } else {
        map.Draft.push(app);
      }
    });

    return map;
  }, [applications]);

  const handleDropApplication = (appId: string, targetStatus: ApplicationStatus) => {
    const app = applications.find((a) => a.id === appId);
    if (!app) return;

    if (app.status === targetStatus) return;

    // Check domain allowedTransitions
    if (app.allowedTransitions && !app.allowedTransitions.includes(targetStatus)) {
      alert(
        `Domain Transition Guard:\nCannot move application from "${app.status}" to "${targetStatus}".\nAllowed transitions from ${app.status}: [${app.allowedTransitions.join(', ') || 'none'}]`
      );
      return;
    }

    onChangeStatus(app, targetStatus);
  };

  const statusesToDisplay =
    activeMobileStatus === 'All'
      ? KANBAN_STATUSES
      : KANBAN_STATUSES.filter((s) => s === activeMobileStatus);

  return (
    <div className="space-y-4">
      {/* Mobile Stage Selector Tabs */}
      <div className="flex lg:hidden overflow-x-auto pb-2 gap-1.5 custom-scrollbar border-b border-border/40">
        <button
          type="button"
          onClick={() => setActiveMobileStatus('All')}
          className={`px-3 py-1.5 text-xs font-semibold rounded-lg shrink-0 transition-colors ${
            activeMobileStatus === 'All'
              ? 'bg-primary text-primary-foreground shadow-sm'
              : 'bg-card text-muted-foreground border border-border hover:text-foreground'
          }`}
        >
          All Stages ({applications.length})
        </button>
        {KANBAN_STATUSES.map((status) => {
          const count = applicationsByStatus[status].length;
          const config = STATUS_CONFIG[status];
          const isSelected = activeMobileStatus === status;
          return (
            <button
              key={status}
              type="button"
              onClick={() => setActiveMobileStatus(status)}
              className={`px-3 py-1.5 text-xs font-semibold rounded-lg shrink-0 flex items-center gap-1.5 transition-colors ${
                isSelected
                  ? 'bg-primary text-primary-foreground shadow-sm'
                  : 'bg-card text-muted-foreground border border-border hover:text-foreground'
              }`}
            >
              <span>{config?.label ?? status}</span>
              <span
                className={`px-1.5 py-0.2 rounded-full text-[10px] font-bold ${
                  isSelected ? 'bg-primary-foreground/20 text-primary-foreground' : 'bg-secondary text-secondary-foreground'
                }`}
              >
                {count}
              </span>
            </button>
          );
        })}
      </div>

      {/* Main Board View */}
      <div className="w-full overflow-x-auto pb-4 custom-scrollbar">
        <div className="flex gap-4 min-w-max pb-2">
          {statusesToDisplay.map((status) => (
            <KanbanColumn
              key={status}
              status={status}
              applications={applicationsByStatus[status]}
              onViewApplication={onViewApplication}
              onChangeStatus={onChangeStatus}
              onDropApplication={handleDropApplication}
            />
          ))}
        </div>
      </div>
    </div>
  );
};

export default KanbanBoard;
