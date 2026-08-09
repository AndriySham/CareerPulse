import React from 'react';
import Modal from '@/components/ui/Modal';
import { useCompany } from '@/api/companies';
import type { VacancyDto } from '@/types';
import { Building2, ExternalLink, Calendar, Edit3, Clock } from 'lucide-react';

interface VacancyDetailModalProps {
  isOpen: boolean;
  onClose: () => void;
  vacancy: VacancyDto | null;
  onEditVacancy: (vacancy: VacancyDto) => void;
}

export const VacancyDetailModal: React.FC<VacancyDetailModalProps> = ({
  isOpen,
  onClose,
  vacancy,
  onEditVacancy,
}) => {
  const { data: company } = useCompany(vacancy?.companyId);

  if (!vacancy) return null;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={vacancy.title}
      description={company ? `Opportunity at ${company.name}` : undefined}
      maxWidth="2xl"
    >
      <div className="space-y-6">
        {/* Meta badges */}
        <div className="flex flex-wrap items-center gap-3 text-xs">
          {company && (
            <span className="inline-flex items-center gap-1.5 rounded-full bg-primary/10 px-3 py-1 font-semibold text-primary border border-primary/20">
              <Building2 className="h-3.5 w-3.5" />
              {company.name}
            </span>
          )}

          {vacancy.postedAt && (
            <span className="inline-flex items-center gap-1.5 rounded-full bg-accent px-3 py-1 font-medium text-muted-foreground">
              <Calendar className="h-3.5 w-3.5 text-primary" />
              Posted: {new Date(vacancy.postedAt).toLocaleDateString()}
            </span>
          )}

          {vacancy.url && (
            <a
              href={vacancy.url}
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-1.5 rounded-full bg-accent px-3 py-1 font-medium text-foreground hover:bg-accent/80 transition-colors"
            >
              <span>View Original Listing</span>
              <ExternalLink className="h-3.5 w-3.5 text-primary" />
            </a>
          )}
        </div>

        {/* Job Description */}
        <div className="space-y-2">
          <h4 className="text-xs font-semibold uppercase tracking-wider text-muted-foreground">
            Job Description & Requirements
          </h4>
          {vacancy.description ? (
            <div className="rounded-xl border border-border/60 bg-muted/20 p-5 text-sm text-foreground whitespace-pre-wrap leading-relaxed max-h-96 overflow-y-auto">
              {vacancy.description}
            </div>
          ) : (
            <div className="rounded-xl border border-dashed border-border p-6 text-center">
              <p className="text-xs text-muted-foreground italic">No detailed description attached to this vacancy.</p>
            </div>
          )}
        </div>

        {/* Footer info & Actions */}
        <div className="flex items-center justify-between border-t border-border/40 pt-4">
          <div className="flex items-center gap-1 text-xs text-muted-foreground">
            <Clock className="h-3.5 w-3.5" />
            <span>Added {new Date(vacancy.createdAt).toLocaleDateString()}</span>
          </div>

          <button
            type="button"
            onClick={() => {
              onClose();
              onEditVacancy(vacancy);
            }}
            className="inline-flex items-center gap-1.5 rounded-lg border border-border bg-card px-3.5 py-1.5 text-xs font-semibold text-foreground hover:bg-accent transition-colors"
          >
            <Edit3 className="h-3.5 w-3.5" /> Edit Vacancy
          </button>
        </div>
      </div>
    </Modal>
  );
};

export default VacancyDetailModal;
