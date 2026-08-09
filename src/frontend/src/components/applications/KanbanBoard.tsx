import React from 'react';
import type { ApplicationDto, ApplicationStatus } from '@/types';
import KanbanColumn from './KanbanColumn';

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
  // Group applications by status
  const applicationsByStatus = React.useMemo(() => {
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

  return (
    <div className="w-full overflow-x-auto pb-4 custom-scrollbar">
      <div className="flex gap-4 min-w-max pb-2">
        {KANBAN_STATUSES.map((status) => (
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
  );
};

export default KanbanBoard;
