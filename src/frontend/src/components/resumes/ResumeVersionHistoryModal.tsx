import React, { useMemo } from 'react';
import Modal from '@/components/ui/Modal';
import ResumeStatusBadge from './ResumeStatusBadge';
import type { ResumeRevisionDto } from '@/types';
import { GitBranch, Calendar, ArrowRight, Award } from 'lucide-react';

interface ResumeVersionHistoryModalProps {
  isOpen: boolean;
  onClose: () => void;
  revisions: ResumeRevisionDto[];
  selectedRevision: ResumeRevisionDto | null;
  onSelectRevision: (revision: ResumeRevisionDto) => void;
}

export const ResumeVersionHistoryModal: React.FC<ResumeVersionHistoryModalProps> = ({
  isOpen,
  onClose,
  revisions,
  selectedRevision,
  onSelectRevision,
}) => {
  // Build revision lineage by tracing parents or matching user name/email family
  const lineage = useMemo(() => {
    if (!selectedRevision) return [];
    // Collect all revisions belonging to the same developer profile (by email or parentId connection)
    const email = selectedRevision.personalInfo?.email?.toLowerCase();
    const family = revisions.filter(
      (r) => r.personalInfo?.email?.toLowerCase() === email
    );

    // Sort by version ascending
    return family.sort((a, b) => a.version - b.version);
  }, [revisions, selectedRevision]);

  if (!selectedRevision) return null;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={`Revision Version History (${selectedRevision.personalInfo?.fullName || 'Profile'})`}
      description="Trace Copy-on-Write version lineage and evolution over time per ADR 005."
      maxWidth="lg"
    >
      <div className="space-y-4">
        {lineage.length === 0 ? (
          <p className="text-xs text-muted-foreground italic text-center py-4">
            No version history found for this revision profile.
          </p>
        ) : (
          <div className="relative pl-6 space-y-6 before:absolute before:left-2.5 before:top-2 before:bottom-2 before:w-0.5 before:bg-border/60">
            {lineage.map((rev) => {
              const isCurrent = rev.id === selectedRevision.id;

              return (
                <div key={rev.id} className="relative flex items-start gap-4 group">
                  {/* Timeline dot */}
                  <div
                    className={`absolute -left-[23px] top-1.5 h-3.5 w-3.5 rounded-full border-2 transition-colors ${
                      isCurrent
                        ? 'border-primary bg-primary shadow-sm shadow-primary/50'
                        : 'border-muted-foreground/40 bg-card group-hover:border-primary'
                    }`}
                  />

                  {/* Card Container */}
                  <div
                    onClick={() => onSelectRevision(rev)}
                    className={`flex-1 rounded-xl border p-4 transition-all cursor-pointer ${
                      isCurrent
                        ? 'border-primary/50 bg-primary/5 shadow-sm'
                        : 'border-border/60 bg-card hover:border-primary/30 hover:bg-accent/30'
                    }`}
                  >
                    <div className="flex items-center justify-between gap-2 mb-2">
                      <div className="flex items-center gap-2">
                        <span className="inline-flex items-center px-2 py-0.5 rounded-md bg-primary/10 text-primary text-xs font-bold border border-primary/20">
                          Version {rev.version}
                        </span>
                        {isCurrent && (
                          <span className="text-[10px] uppercase font-bold text-primary tracking-wider bg-primary/20 px-1.5 py-0.5 rounded">
                            Selected
                          </span>
                        )}
                        {rev.parentRevisionId && (
                          <span className="inline-flex items-center gap-1 text-[11px] text-muted-foreground">
                            <GitBranch className="h-3 w-3" />
                            From parent
                          </span>
                        )}
                      </div>

                      <ResumeStatusBadge status={rev.status} size="sm" />
                    </div>

                    <p className="text-xs text-muted-foreground line-clamp-2 italic bg-accent/20 p-2 rounded border border-border/30 my-2">
                      "{rev.professionalSummary}"
                    </p>

                    <div className="flex items-center justify-between text-[11px] text-muted-foreground pt-1 border-t border-border/30">
                      <div className="flex items-center gap-1">
                        <Calendar className="h-3 w-3 text-primary/70" />
                        <span>{new Date(rev.createdAt).toLocaleDateString()}</span>
                      </div>

                      <div className="flex items-center gap-2">
                        <span className="flex items-center gap-1">
                          <Award className="h-3 w-3 text-primary/70" />
                          {rev.skills?.length || 0} skills
                        </span>
                        <span className="flex items-center gap-0.5 text-primary font-semibold">
                          View <ArrowRight className="h-3 w-3" />
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        )}

        <div className="flex items-center justify-end pt-3 border-t border-border/40">
          <button
            type="button"
            onClick={onClose}
            className="rounded-lg border border-border bg-background px-4 py-2 text-sm font-semibold text-foreground hover:bg-accent transition-colors"
          >
            Close
          </button>
        </div>
      </div>
    </Modal>
  );
};

export default ResumeVersionHistoryModal;
