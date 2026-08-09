import React from 'react';
import type { CompanyDto } from '@/types';
import { useVacancies } from '@/api/vacancies';
import { Building2, Globe, Tag, Edit3, Plus, ExternalLink, Briefcase, ChevronRight } from 'lucide-react';

interface CompanyCardProps {
  company: CompanyDto;
  onView: (company: CompanyDto) => void;
  onEdit: (company: CompanyDto) => void;
  onAddVacancy: (companyId: string) => void;
}

export const CompanyCard: React.FC<CompanyCardProps> = ({
  company,
  onView,
  onEdit,
  onAddVacancy,
}) => {
  const { data: vacancies = [] } = useVacancies(company.id);

  return (
    <div className="group relative flex flex-col justify-between rounded-xl border border-border/60 bg-card p-5 shadow-sm transition-all hover:border-primary/40 hover:shadow-md">
      <div>
        {/* Top bar: Icon, Name, Archived Badge */}
        <div className="flex items-start justify-between gap-3">
          <div className="flex items-center gap-3">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary font-bold">
              <Building2 className="h-5 w-5" />
            </div>
            <div>
              <h3
                onClick={() => onView(company)}
                className="font-bold text-base text-foreground tracking-tight hover:text-primary transition-colors cursor-pointer"
              >
                {company.name}
              </h3>
              {company.industry ? (
                <span className="inline-flex items-center gap-1 text-xs text-muted-foreground mt-0.5">
                  <Tag className="h-3 w-3 text-primary/70" /> {company.industry}
                </span>
              ) : (
                <span className="text-xs text-muted-foreground/60 italic mt-0.5 block">No industry set</span>
              )}
            </div>
          </div>

          {company.isArchived && (
            <span className="rounded-md bg-muted px-2 py-0.5 text-[11px] font-semibold text-muted-foreground border border-border">
              Archived
            </span>
          )}
        </div>

        {/* Website link & Notes preview */}
        <div className="mt-4 space-y-2 text-xs">
          {company.website && (
            <div className="flex items-center gap-1.5 text-muted-foreground">
              <Globe className="h-3.5 w-3.5 shrink-0 text-primary/80" />
              <a
                href={company.website}
                target="_blank"
                rel="noopener noreferrer"
                className="hover:underline hover:text-primary truncate"
                onClick={(e) => e.stopPropagation()}
              >
                {company.website.replace(/^https?:\/\//, '')}
              </a>
              <ExternalLink className="h-3 w-3 shrink-0" />
            </div>
          )}

          {company.notes ? (
            <p className="line-clamp-2 text-muted-foreground text-xs leading-relaxed pt-1">
              {company.notes}
            </p>
          ) : (
            <p className="text-xs text-muted-foreground/40 italic pt-1">No notes</p>
          )}
        </div>
      </div>

      {/* Footer bar */}
      <div className="mt-5 flex items-center justify-between border-t border-border/40 pt-3">
        <div className="flex items-center gap-1.5 text-xs text-muted-foreground font-medium">
          <Briefcase className="h-3.5 w-3.5 text-primary/80" />
          <span>{vacancies.length} {vacancies.length === 1 ? 'vacancy' : 'vacancies'}</span>
        </div>

        <div className="flex items-center gap-1">
          <button
            type="button"
            onClick={() => onAddVacancy(company.id)}
            className="rounded-lg p-1.5 text-muted-foreground hover:bg-accent hover:text-primary transition-colors"
            title="Add Vacancy"
          >
            <Plus className="h-4 w-4" />
          </button>
          <button
            type="button"
            onClick={() => onEdit(company)}
            className="rounded-lg p-1.5 text-muted-foreground hover:bg-accent hover:text-primary transition-colors"
            title="Edit Company"
          >
            <Edit3 className="h-4 w-4" />
          </button>
          <button
            type="button"
            onClick={() => onView(company)}
            className="inline-flex items-center gap-1 rounded-lg bg-accent/60 px-2.5 py-1 text-xs font-semibold text-foreground hover:bg-accent transition-colors"
          >
            View <ChevronRight className="h-3.5 w-3.5" />
          </button>
        </div>
      </div>
    </div>
  );
};

export default CompanyCard;
