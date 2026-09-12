import React from 'react';
import Modal from '@/components/ui/Modal';
import { useVacancies } from '@/api/vacancies';
import { formatSalary, formatWorkMode, formatEmploymentType } from '@/types';
import type { CompanyDto } from '@/types';
import {
  Globe,
  Tag,
  Edit3,
  Plus,
  Briefcase,
  ExternalLink,
  Calendar,
  Archive,
  Laptop,
  MapPin,
  Banknote,
} from 'lucide-react';

interface CompanyDetailModalProps {
  isOpen: boolean;
  onClose: () => void;
  company: CompanyDto | null;
  onEditCompany: (company: CompanyDto) => void;
  onAddVacancy: (companyId: string) => void;
  onSelectVacancy?: (vacancyId: string) => void;
}

export const CompanyDetailModal: React.FC<CompanyDetailModalProps> = ({
  isOpen,
  onClose,
  company,
  onEditCompany,
  onAddVacancy,
  onSelectVacancy,
}) => {
  const companyId = company?.id;
  const { data: vacancies = [], isLoading: vacanciesLoading } = useVacancies(companyId, Boolean(companyId));

  if (!company) return null;

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={company.name}
      maxWidth="2xl"
    >
      <div className="space-y-6">
        {/* Header badges & Quick Info */}
        <div className="flex flex-wrap items-center gap-3 text-xs">
          {company.isArchived ? (
            <span className="inline-flex items-center gap-1 rounded-full bg-muted px-2.5 py-1 font-semibold text-muted-foreground">
              <Archive className="h-3.5 w-3.5" /> Archived
            </span>
          ) : (
            <span className="inline-flex items-center gap-1 rounded-full bg-emerald-500/10 px-2.5 py-1 font-semibold text-emerald-500 border border-emerald-500/20">
              Active Company
            </span>
          )}

          {company.industry && (
            <span className="inline-flex items-center gap-1 rounded-full bg-primary/10 px-2.5 py-1 font-medium text-primary border border-primary/20">
              <Tag className="h-3.5 w-3.5" /> {company.industry}
            </span>
          )}

          {company.website && (
            <a
              href={company.website}
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center gap-1 rounded-full bg-accent px-2.5 py-1 font-medium text-foreground hover:bg-accent/80 transition-colors"
            >
              <Globe className="h-3.5 w-3.5 text-muted-foreground" />
              <span>{company.website.replace(/^https?:\/\//, '')}</span>
              <ExternalLink className="h-3 w-3 text-muted-foreground ml-0.5" />
            </a>
          )}
        </div>

        {/* Notes section */}
        {company.notes ? (
          <div className="rounded-lg border border-border/60 bg-muted/30 p-4">
            <h4 className="text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
              Internal Notes
            </h4>
            <p className="text-sm text-foreground whitespace-pre-wrap leading-relaxed">
              {company.notes}
            </p>
          </div>
        ) : (
          <p className="text-xs text-muted-foreground italic">No internal notes added yet.</p>
        )}

        {/* Associated Vacancies Section */}
        <div className="border-t border-border/40 pt-4 space-y-3">
          <div className="flex items-center justify-between">
            <h3 className="text-sm font-bold tracking-tight text-foreground flex items-center gap-2">
              <Briefcase className="h-4 w-4 text-primary" />
              Vacancies ({vacancies.length})
            </h3>
            <button
              type="button"
              onClick={() => {
                onClose();
                onAddVacancy(company.id);
              }}
              className="inline-flex items-center gap-1 text-xs font-semibold text-primary hover:text-primary/80 transition-colors"
            >
              <Plus className="h-3.5 w-3.5" /> Add Vacancy
            </button>
          </div>

          {vacanciesLoading ? (
            <div className="space-y-2">
              <div className="h-12 rounded-lg bg-accent/40 animate-pulse" />
              <div className="h-12 rounded-lg bg-accent/40 animate-pulse" />
            </div>
          ) : vacancies.length === 0 ? (
            <div className="rounded-lg border border-dashed border-border p-4 text-center">
              <p className="text-xs text-muted-foreground">No open or historical vacancies for this company.</p>
            </div>
          ) : (
            <div className="space-y-2 max-h-60 overflow-y-auto pr-1">
              {vacancies.map((v) => {
                const salaryStr = formatSalary(v.salaryMin, v.salaryMax, v.salaryCurrency);
                const workModeStr = formatWorkMode(v.workMode);
                const employmentTypeStr = formatEmploymentType(v.employmentType);
                return (
                  <div
                    key={v.id}
                    onClick={() => onSelectVacancy?.(v.id)}
                    className="flex items-center justify-between rounded-lg border border-border/40 bg-card p-3 hover:bg-accent/40 transition-colors cursor-pointer"
                  >
                    <div className="space-y-1">
                      <h4 className="text-sm font-semibold text-foreground">{v.title}</h4>
                      <div className="flex flex-wrap items-center gap-2 text-[11px] text-muted-foreground">
                        {workModeStr && (
                          <span className="inline-flex items-center gap-0.5 rounded bg-accent px-1.5 py-0.5 font-medium text-foreground">
                            <Laptop className="h-3 w-3 text-primary" /> {workModeStr}
                          </span>
                        )}
                        {employmentTypeStr && (
                          <span className="inline-flex items-center gap-0.5 rounded bg-accent px-1.5 py-0.5 font-medium text-foreground">
                            <Briefcase className="h-3 w-3 text-primary" /> {employmentTypeStr}
                          </span>
                        )}
                        {v.location && (
                          <span className="inline-flex items-center gap-0.5">
                            <MapPin className="h-3 w-3 text-primary" /> {v.location}
                          </span>
                        )}
                        {salaryStr && (
                          <span className="inline-flex items-center gap-0.5 text-emerald-400 font-semibold">
                            <Banknote className="h-3 w-3" /> {salaryStr}
                          </span>
                        )}
                        {v.postedAt && (
                          <span className="flex items-center gap-1">
                            <Calendar className="h-3 w-3" />
                            Posted: {new Date(v.postedAt).toLocaleDateString()}
                          </span>
                        )}
                      </div>
                    </div>
                    {v.url && (
                      <a
                        href={v.url}
                        target="_blank"
                        rel="noopener noreferrer"
                        onClick={(e) => e.stopPropagation()}
                        className="p-1.5 rounded-md text-muted-foreground hover:text-primary hover:bg-accent transition-colors shrink-0 ml-2"
                        title="Open job posting"
                      >
                        <ExternalLink className="h-4 w-4" />
                      </a>
                    )}
                  </div>
                );
              })}
            </div>
          )}
        </div>

        {/* Footer actions */}
        <div className="flex items-center justify-between border-t border-border/40 pt-4">
          <div className="text-xs text-muted-foreground">
            Created: {new Date(company.createdAt).toLocaleDateString()}
          </div>

          <div className="flex items-center gap-2">
            <button
              type="button"
              onClick={() => {
                onClose();
                onEditCompany(company);
              }}
              className="inline-flex items-center gap-1.5 rounded-lg border border-border bg-card px-3 py-1.5 text-xs font-semibold text-foreground hover:bg-accent transition-colors"
            >
              <Edit3 className="h-3.5 w-3.5" /> Edit Company
            </button>
          </div>
        </div>
      </div>
    </Modal>
  );
};

export default CompanyDetailModal;
