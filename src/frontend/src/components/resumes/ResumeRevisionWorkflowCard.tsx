import React, { useMemo, useState } from 'react';
import type { ResumeRevisionDto } from '@/types';
import ResumeStatusBadge from './ResumeStatusBadge';
import { GitBranch, Plus, Loader2, History, Calendar, ChevronDown, ChevronUp } from 'lucide-react';

interface ResumeRevisionWorkflowCardProps {
  currentRevision: ResumeRevisionDto;
  allRevisions: ResumeRevisionDto[];
  onSelectRevision: (revisionId: string) => void;
  onSpawnVersion: () => void;
  onOpenHistoryModal?: () => void;
  isSpawning?: boolean;
}

export const ResumeRevisionWorkflowCard: React.FC<ResumeRevisionWorkflowCardProps> = ({
  currentRevision,
  allRevisions,
  onSelectRevision,
  onSpawnVersion,
  onOpenHistoryModal,
  isSpawning = false,
}) => {
  const [showCompactHistory, setShowCompactHistory] = useState(false);

  // Lineage of revisions for this parent Resume profile, sorted descending by version
  const lineageDescending = useMemo(() => {
    const family = allRevisions.filter(
      (r) =>
        r.resumeId === currentRevision.resumeId ||
        (r.personalInfo?.email &&
          r.personalInfo.email.toLowerCase() ===
            currentRevision.personalInfo?.email?.toLowerCase())
    );
    return family.sort((a, b) => b.version - a.version);
  }, [allRevisions, currentRevision]);

  const formatDateShort = (dateString: string) => {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
    } catch {
      return '';
    }
  };

  return (
    <div className="rounded-xl border border-border/60 bg-card p-5 shadow-sm space-y-4">
      {/* Current Revision Header Bar */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 border-b border-border/40 pb-3">
        <div className="flex items-center gap-3">
          <div className="p-2 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <GitBranch className="h-5 w-5" />
          </div>
          <div>
            <div className="flex items-center gap-2">
              <h3 className="text-base font-bold text-foreground">
                Revision {currentRevision.version}
              </h3>
              <ResumeStatusBadge status={currentRevision.status} size="sm" />
              {currentRevision.status === 'Draft' && (
                <span className="text-[10px] font-semibold text-emerald-400 bg-emerald-500/10 px-2 py-0.5 rounded-full border border-emerald-500/20">
                  Current Draft
                </span>
              )}
            </div>
            <p className="text-xs text-muted-foreground">
              {currentRevision.personalInfo?.fullName
                ? `${currentRevision.personalInfo.fullName}'s Resume`
                : 'Software Developer Resume'}{' '}
              — Version {currentRevision.version} snapshot
            </p>
          </div>
        </div>

        {/* Header Actions */}
        <div className="flex items-center gap-2">
          {currentRevision.status === 'Applied' && (
            <button
              type="button"
              onClick={onSpawnVersion}
              disabled={isSpawning}
              className="inline-flex items-center gap-1.5 rounded-lg bg-primary/10 text-primary border border-primary/20 px-3 py-1.5 text-xs font-semibold hover:bg-primary/20 transition-all cursor-pointer disabled:opacity-50"
              title="Create a new draft version from this revision"
            >
              {isSpawning ? (
                <Loader2 className="h-3.5 w-3.5 animate-spin" />
              ) : (
                <Plus className="h-3.5 w-3.5" />
              )}
              <span>Create New Version</span>
            </button>
          )}

          <button
            type="button"
            onClick={() => {
              if (onOpenHistoryModal) {
                onOpenHistoryModal();
              } else {
                setShowCompactHistory(!showCompactHistory);
              }
            }}
            className="inline-flex items-center gap-1.5 rounded-lg border border-border bg-background px-3 py-1.5 text-xs font-semibold text-foreground hover:bg-accent transition-colors cursor-pointer"
            title="View revision history"
          >
            <History className="h-3.5 w-3.5 text-muted-foreground" />
            <span>View History</span>
            {showCompactHistory ? (
              <ChevronUp className="h-3 w-3 text-muted-foreground" />
            ) : (
              <ChevronDown className="h-3 w-3 text-muted-foreground" />
            )}
          </button>
        </div>
      </div>

      {/* Revision Switcher Lineage */}
      <div className="space-y-2">
        <div className="flex items-center justify-between text-[11px] font-medium text-muted-foreground">
          <span>Revisions Lineage:</span>
          <span>{lineageDescending.length} version(s) available</span>
        </div>
        <div className="flex items-center gap-2 overflow-x-auto pb-1">
          {lineageDescending.map((rev) => {
            const isCurrent = rev.id === currentRevision.id;
            return (
              <button
                key={rev.id}
                type="button"
                onClick={() => onSelectRevision(rev.id)}
                className={`flex items-center gap-2 px-3 py-1.5 rounded-xl text-xs font-semibold border transition-all whitespace-nowrap cursor-pointer ${
                  isCurrent
                    ? 'border-primary bg-primary/10 text-primary shadow-xs ring-1 ring-primary/30'
                    : 'border-border/60 bg-background text-muted-foreground hover:bg-accent hover:text-foreground'
                }`}
              >
                <span className="font-bold">v{rev.version}</span>
                <ResumeStatusBadge status={rev.status} size="sm" />
                {isCurrent && (
                  <span className="text-[10px] text-primary font-semibold uppercase tracking-wider">
                    current
                  </span>
                )}
              </button>
            );
          })}
        </div>
      </div>

      {/* Compact Revision History Area (Collapsible / Expandable) */}
      {showCompactHistory && (
        <div className="rounded-xl border border-border/60 bg-background p-4 space-y-3 mt-2 animate-in fade-in duration-200">
          <div className="flex items-center justify-between border-b border-border/40 pb-2">
            <h4 className="text-xs font-bold text-foreground uppercase tracking-wider flex items-center gap-1.5">
              <History className="h-3.5 w-3.5 text-primary" />
              Revision History
            </h4>
            <span className="text-[11px] text-muted-foreground font-mono">
              v{currentRevision.version} active
            </span>
          </div>

          <div className="space-y-1.5 max-h-48 overflow-y-auto pr-1">
            {lineageDescending.map((rev) => {
              const isCurrent = rev.id === currentRevision.id;
              const dateStr = formatDateShort(rev.createdAt);

              return (
                <div
                  key={rev.id}
                  onClick={() => onSelectRevision(rev.id)}
                  className={`flex items-center justify-between p-2.5 rounded-lg border transition-all cursor-pointer ${
                    isCurrent
                      ? 'border-primary/40 bg-primary/5 text-foreground font-semibold'
                      : 'border-border/40 bg-card hover:bg-accent/40 text-muted-foreground hover:text-foreground'
                  }`}
                >
                  <div className="flex items-center gap-3">
                    <span className="font-bold text-xs w-6 text-center">v{rev.version}</span>
                    <ResumeStatusBadge status={rev.status} size="sm" />
                    {isCurrent && (
                      <span className="text-[10px] bg-primary/20 text-primary font-bold px-1.5 py-0.5 rounded">
                        current revision
                      </span>
                    )}
                  </div>

                  <div className="flex items-center gap-3 text-xs">
                    <span className="text-muted-foreground flex items-center gap-1">
                      <Calendar className="h-3 w-3" />
                      {dateStr}
                    </span>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
};

export default ResumeRevisionWorkflowCard;
