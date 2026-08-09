import React, { useState } from 'react';
import type { ApplicationDto, ApplicationStatus } from '@/types';
import ApplicationStatusBadge, { STATUS_CONFIG } from './ApplicationStatusBadge';
import {
  Building2,
  Briefcase,
  Globe,
  Calendar,
  ChevronRight,
  MoreVertical,
  ArrowRight,
  FileText,
} from 'lucide-react';

interface ApplicationCardProps {
  application: ApplicationDto;
  onView: (application: ApplicationDto) => void;
  onChangeStatus: (application: ApplicationDto, newStatus: ApplicationStatus) => void;
  isDragging?: boolean;
}

export const ApplicationCard: React.FC<ApplicationCardProps> = ({
  application,
  onView,
  onChangeStatus,
}) => {
  const [showMenu, setShowMenu] = useState(false);

  const formattedDate = application.appliedAt
    ? new Date(application.appliedAt).toLocaleDateString(undefined, {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
      })
    : new Date(application.createdAt).toLocaleDateString(undefined, {
        month: 'short',
        day: 'numeric',
        year: 'numeric',
      });

  const handleDragStart = (e: React.DragEvent) => {
    e.dataTransfer.setData('text/plain', application.id);
    e.dataTransfer.setData('application/json', JSON.stringify(application));
    e.dataTransfer.effectAllowed = 'move';
  };

  return (
    <div
      draggable
      onDragStart={handleDragStart}
      className="group relative flex flex-col justify-between rounded-xl border border-border/60 bg-card p-4 shadow-sm transition-all hover:border-primary/50 hover:shadow-md cursor-grab active:cursor-grabbing"
    >
      <div>
        {/* Header: Company Name + Action Menu */}
        <div className="flex items-start justify-between gap-2">
          <div className="flex items-center gap-2.5 min-w-0">
            <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary font-bold">
              <Building2 className="h-4 w-4" />
            </div>
            <div className="min-w-0">
              <h4
                onClick={() => onView(application)}
                className="font-bold text-sm text-foreground tracking-tight hover:text-primary transition-colors cursor-pointer truncate"
              >
                {application.companyName}
              </h4>
              {application.vacancyTitle ? (
                <div className="flex items-center gap-1 text-xs text-muted-foreground truncate mt-0.5">
                  <Briefcase className="h-3 w-3 shrink-0 text-primary/70" />
                  <span className="truncate">{application.vacancyTitle}</span>
                </div>
              ) : (
                <span className="text-xs text-muted-foreground/50 italic mt-0.5 block">
                  General Application
                </span>
              )}
            </div>
          </div>

          {/* Quick transition dropdown / menu */}
          <div className="relative">
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation();
                setShowMenu(!showMenu);
              }}
              className="rounded-lg p-1 text-muted-foreground hover:bg-accent hover:text-foreground transition-colors"
              title="Change Status"
            >
              <MoreVertical className="h-4 w-4" />
            </button>

            {showMenu && (
              <>
                <div
                  className="fixed inset-0 z-20"
                  onClick={(e) => {
                    e.stopPropagation();
                    setShowMenu(false);
                  }}
                />
                <div className="absolute right-0 top-7 z-30 w-48 rounded-xl border border-border bg-card p-1.5 shadow-xl animate-in fade-in zoom-in-95 duration-150">
                  <div className="px-2 py-1 text-[11px] font-semibold text-muted-foreground border-b border-border/40 mb-1">
                    Allowed Status Transitions
                  </div>
                  {application.allowedTransitions && application.allowedTransitions.length > 0 ? (
                    application.allowedTransitions.map((status) => {
                      const statusInfo = STATUS_CONFIG[status];
                      return (
                        <button
                          key={status}
                          type="button"
                          onClick={(e) => {
                            e.stopPropagation();
                            setShowMenu(false);
                            onChangeStatus(application, status);
                          }}
                          className="w-full flex items-center justify-between px-2 py-1.5 text-xs font-medium rounded-lg hover:bg-primary/10 hover:text-primary transition-colors"
                        >
                          <span className="flex items-center gap-1.5">
                            <ArrowRight className="h-3 w-3 text-muted-foreground" />
                            {statusInfo?.label ?? status}
                          </span>
                        </button>
                      );
                    })
                  ) : (
                    <div className="px-2 py-1.5 text-xs text-muted-foreground/60 italic">
                      Terminal state (no transitions)
                    </div>
                  )}
                </div>
              </>
            )}
          </div>
        </div>

        {/* Status & Metadata */}
        <div className="mt-3 flex flex-wrap items-center gap-2">
          <ApplicationStatusBadge status={application.status} size="sm" />
          <span className="inline-flex items-center gap-1 rounded-md bg-secondary/80 px-2 py-0.5 text-[11px] font-medium text-secondary-foreground">
            <Globe className="h-3 w-3 text-muted-foreground" />
            {application.jobSource}
          </span>
        </div>

        {/* Notes preview if present */}
        {application.notes && (
          <div className="mt-2.5 flex items-start gap-1.5 text-xs text-muted-foreground/80 bg-accent/40 rounded-lg p-2">
            <FileText className="h-3.5 w-3.5 shrink-0 text-muted-foreground mt-0.5" />
            <p className="line-clamp-2 leading-tight">{application.notes}</p>
          </div>
        )}
      </div>

      {/* Footer: Date & Details Button */}
      <div className="mt-3.5 flex items-center justify-between border-t border-border/40 pt-2.5 text-xs">
        <span className="flex items-center gap-1 text-[11px] text-muted-foreground">
          <Calendar className="h-3 w-3" />
          {application.appliedAt ? `Applied ${formattedDate}` : `Created ${formattedDate}`}
        </span>

        <button
          type="button"
          onClick={() => onView(application)}
          className="inline-flex items-center gap-1 rounded-lg px-2 py-1 text-xs font-semibold text-primary hover:bg-primary/10 transition-colors"
        >
          Details <ChevronRight className="h-3.5 w-3.5" />
        </button>
      </div>
    </div>
  );
};

export default ApplicationCard;
