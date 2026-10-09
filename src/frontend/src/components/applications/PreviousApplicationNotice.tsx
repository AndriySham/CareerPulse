import React, { useMemo } from 'react';
import { Link } from 'react-router-dom';
import { useApplications } from '@/api/applications';
import ApplicationStatusBadge from '@/components/applications/ApplicationStatusBadge';
import { cn } from '@/lib/utils';
import type { ApplicationDto } from '@/types';
import {
  Info,
  Calendar,
  Globe,
  AlertTriangle,
  Loader2,
  ExternalLink,
} from 'lucide-react';

export interface PreviousApplicationNoticeProps {
  vacancyId?: string | null;
  onViewApplication?: (application: ApplicationDto) => void;
  className?: string;
}

function formatSubmissionDate(appliedAt?: string | null, createdAt?: string): {
  label: string;
  formattedDate: string;
} {
  if (appliedAt) {
    const d = new Date(appliedAt);
    if (!Number.isNaN(d.getTime())) {
      return {
        label: 'Submitted:',
        formattedDate: d.toLocaleDateString(undefined, {
          year: 'numeric',
          month: 'short',
          day: 'numeric',
        }),
      };
    }
  }

  if (createdAt) {
    const d = new Date(createdAt);
    if (!Number.isNaN(d.getTime())) {
      return {
        label: 'Created:',
        formattedDate: `${d.toLocaleDateString(undefined, {
          year: 'numeric',
          month: 'short',
          day: 'numeric',
        })} (Draft)`,
      };
    }
  }

  return { label: 'Date:', formattedDate: 'N/A' };
}

function getEffectiveTimestamp(app: ApplicationDto): number {
  if (app.appliedAt) {
    const t = new Date(app.appliedAt).getTime();
    if (!Number.isNaN(t)) return t;
  }
  if (app.createdAt) {
    const t = new Date(app.createdAt).getTime();
    if (!Number.isNaN(t)) return t;
  }
  return 0;
}

export const PreviousApplicationNotice: React.FC<PreviousApplicationNoticeProps> = ({
  vacancyId,
  onViewApplication,
  className,
}) => {
  const isEnabled = Boolean(vacancyId);

  const {
    data: applications = [],
    isLoading,
    isError,
  } = useApplications(
    isEnabled ? { vacancyId: vacancyId! } : undefined,
    isEnabled
  );

  // Exact matching strictly by VacancyId
  const matchingApplications = useMemo(() => {
    if (!vacancyId) return [];
    return applications.filter((app) => app.vacancyId === vacancyId);
  }, [applications, vacancyId]);

  // Select most recent application using appliedAt (SubmissionDate) with createdAt as fallback
  const mostRecentApplication = useMemo(() => {
    if (matchingApplications.length === 0) return null;
    return [...matchingApplications].sort(
      (a, b) => getEffectiveTimestamp(b) - getEffectiveTimestamp(a)
    )[0];
  }, [matchingApplications]);

  if (!isEnabled) {
    return null;
  }

  // Non-blocking loading state
  if (isLoading) {
    return (
      <div
        role="status"
        aria-live="polite"
        className={cn(
          'rounded-xl border border-border/60 bg-muted/20 p-4 text-xs text-muted-foreground flex items-center gap-3 animate-pulse',
          className
        )}
      >
        <Loader2 className="h-4 w-4 animate-spin text-primary shrink-0" />
        <span>Checking for previous applications for this vacancy...</span>
      </div>
    );
  }

  // Non-blocking error state
  if (isError) {
    return (
      <div
        role="alert"
        className={cn(
          'rounded-xl border border-amber-500/30 bg-amber-500/10 p-4 text-xs text-amber-300 flex items-start gap-3',
          className
        )}
      >
        <AlertTriangle className="h-4 w-4 shrink-0 text-amber-400 mt-0.5" />
        <div className="space-y-0.5">
          <span className="font-semibold block text-amber-300">
            Notice: Unable to check previous applications
          </span>
          <span className="text-muted-foreground block text-[11px]">
            We couldn&apos;t verify if you previously applied for this vacancy. You can still proceed with submitting your application.
          </span>
        </div>
      </div>
    );
  }

  // If no previous application exists, keep the form unchanged
  if (!mostRecentApplication) {
    return null;
  }

  const { label: dateLabel, formattedDate } = formatSubmissionDate(
    mostRecentApplication.appliedAt,
    mostRecentApplication.createdAt
  );

  return (
    <div
      role="region"
      aria-label="Previous application notice"
      className={cn(
        'rounded-xl border border-blue-500/30 bg-blue-500/10 p-4 text-xs space-y-3',
        className
      )}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="flex items-start gap-3">
          <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-blue-500/20 text-blue-400 font-bold mt-0.5">
            <Info className="h-4 w-4" />
          </div>
          <div className="space-y-1">
            <div className="flex items-center gap-2 flex-wrap">
              <span className="font-bold text-foreground text-sm">
                Previous Application Found
              </span>
              {matchingApplications.length > 1 && (
                <span className="text-[11px] font-medium text-muted-foreground bg-accent px-2 py-0.5 rounded-full border border-border/50">
                  {matchingApplications.length} previous submissions
                </span>
              )}
            </div>
            <p className="text-muted-foreground text-xs leading-relaxed">
              You have previously submitted an application for this vacancy. You can review your previous submission below or proceed to submit a new application.
            </p>
          </div>
        </div>
      </div>

      {/* Summary Card with Status, Submission Date, Channel */}
      <div className="rounded-lg bg-card/80 border border-border/60 p-3 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
        <div className="flex flex-wrap items-center gap-x-4 gap-y-2 text-xs">
          {/* Status */}
          <div className="flex items-center gap-1.5">
            <span className="text-muted-foreground font-medium">Status:</span>
            <ApplicationStatusBadge status={mostRecentApplication.status} size="sm" />
          </div>

          {/* Submission Date */}
          <div className="flex items-center gap-1.5">
            <span className="text-muted-foreground font-medium">{dateLabel}</span>
            <span className="font-semibold text-foreground flex items-center gap-1">
              <Calendar className="h-3.5 w-3.5 text-primary" />
              {formattedDate}
            </span>
          </div>

          {/* Job Source / Channel */}
          <div className="flex items-center gap-1.5">
            <span className="text-muted-foreground font-medium">Channel:</span>
            <span className="font-semibold text-foreground flex items-center gap-1">
              <Globe className="h-3.5 w-3.5 text-primary" />
              {mostRecentApplication.jobSource || 'Not specified'}
            </span>
          </div>
        </div>

        {/* View Previous Application Action */}
        {onViewApplication ? (
          <button
            type="button"
            onClick={() => onViewApplication(mostRecentApplication)}
            className="inline-flex items-center gap-1.5 rounded-lg border border-border bg-card px-3 py-1.5 text-xs font-semibold text-foreground hover:bg-accent hover:text-primary transition-colors cursor-pointer shrink-0 self-start sm:self-auto"
          >
            <span>View Previous Application</span>
            <ExternalLink className="h-3.5 w-3.5" />
          </button>
        ) : (
          <Link
            to="/applications"
            className="inline-flex items-center gap-1.5 rounded-lg border border-border bg-card px-3 py-1.5 text-xs font-semibold text-foreground hover:bg-accent hover:text-primary transition-colors shrink-0 self-start sm:self-auto"
          >
            <span>View on Applications Board</span>
            <ExternalLink className="h-3.5 w-3.5" />
          </Link>
        )}
      </div>
    </div>
  );
};

export default PreviousApplicationNotice;
