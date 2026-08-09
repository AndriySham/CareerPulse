import React, { useState, useEffect } from 'react';
import { Modal } from '@/components/ui/Modal';
import { ErrorAlert } from '@/components/ui/ErrorAlert';
import { useCompanies } from '@/api/companies';
import { useVacancies } from '@/api/vacancies';
import { useResumeRevisions } from '@/api/resumes';
import { useSubmitApplication } from '@/api/applications';
import CustomSelect from '@/components/ui/CustomSelect';
import type { SubmitApplicationDto } from '@/types';
import { Building2, Briefcase, FileText, Globe, Send, AlertCircle } from 'lucide-react';

interface ApplicationFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  initialCompanyId?: string;
  initialVacancyId?: string;
}

const COMMON_SOURCES = ['LinkedIn', 'Djinni', 'DOU', 'Company Site', 'Referral', 'Glassdoor', 'Indeed'];

export const ApplicationFormModal: React.FC<ApplicationFormModalProps> = ({
  isOpen,
  onClose,
  initialCompanyId = '',
  initialVacancyId = '',
}) => {
  const [companyId, setCompanyId] = useState(initialCompanyId);
  const [vacancyId, setVacancyId] = useState(initialVacancyId);
  const [resumeRevisionId, setResumeRevisionId] = useState('');
  const [jobSource, setJobSource] = useState('LinkedIn');
  const [customJobSource, setCustomJobSource] = useState('');
  const [notes, setNotes] = useState('');
  const [submitImmediately, setSubmitImmediately] = useState(true);

  // Queries
  const { data: companies = [], isLoading: isLoadingCompanies } = useCompanies();
  const { data: vacancies = [], isLoading: isLoadingVacancies } = useVacancies(companyId || undefined, Boolean(companyId));
  const { data: resumeRevisions = [], isLoading: isLoadingResumes } = useResumeRevisions();

  // Mutation
  const submitApplicationMutation = useSubmitApplication();

  useEffect(() => {
    if (isOpen) {
      setCompanyId(initialCompanyId);
      setVacancyId(initialVacancyId);
      setJobSource('LinkedIn');
      setCustomJobSource('');
      setNotes('');
      setSubmitImmediately(true);
      submitApplicationMutation.reset();

      // Auto-select first resume revision if available
      if (resumeRevisions.length > 0 && !resumeRevisionId) {
        setResumeRevisionId(resumeRevisions[0].id);
      }
    }
  }, [isOpen, initialCompanyId, initialVacancyId, resumeRevisions]);

  // Sync resume revision selection if loaded after modal opened
  useEffect(() => {
    if (resumeRevisions.length > 0 && !resumeRevisionId) {
      setResumeRevisionId(resumeRevisions[0].id);
    }
  }, [resumeRevisions, resumeRevisionId]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const selectedSource = jobSource === 'Custom' ? customJobSource.trim() : jobSource;

    if (!companyId) return;
    if (!resumeRevisionId) return;
    if (!selectedSource) return;

    const dto: SubmitApplicationDto = {
      companyId,
      vacancyId: vacancyId || null,
      resumeRevisionId,
      jobSource: selectedSource,
      notes: notes.trim() || null,
      submitImmediately,
    };

    submitApplicationMutation.mutate(dto, {
      onSuccess: () => {
        onClose();
      },
    });
  };

  const isFormValid = Boolean(
    companyId &&
    resumeRevisionId &&
    (jobSource !== 'Custom' || customJobSource.trim().length > 0)
  );

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Submit Job Application"
      description="Create a new application entry and track its progress in your Kanban pipeline."
      maxWidth="lg"
    >
      <form onSubmit={handleSubmit} autoComplete="off" className="space-y-4">
        {submitApplicationMutation.isError && (
          <ErrorAlert error={submitApplicationMutation.error} />
        )}

        {/* Company Select */}
        <div>
          <label className="block text-xs font-semibold text-foreground mb-1.5 flex items-center gap-1.5">
            <Building2 className="h-3.5 w-3.5 text-primary" /> Target Company <span className="text-destructive">*</span>
          </label>
          <CustomSelect
            value={companyId}
            onChange={(val) => {
              setCompanyId(val);
              setVacancyId(''); // Reset vacancy selection on company change
            }}
            disabled={isLoadingCompanies}
            placeholder="Select a company..."
            options={companies.map((company) => ({
              value: company.id,
              label: `${company.name}${company.industry ? ` (${company.industry})` : ''}`,
            }))}
            className="w-full"
          />
          {companies.length === 0 && !isLoadingCompanies && (
            <p className="mt-1 text-xs text-amber-500 flex items-center gap-1">
              <AlertCircle className="h-3 w-3" /> No companies found. Please add a company in Companies section first.
            </p>
          )}
        </div>

        {/* Vacancy Select (Optional) */}
        <div>
          <label className="block text-xs font-semibold text-foreground mb-1.5 flex items-center gap-1.5">
            <Briefcase className="h-3.5 w-3.5 text-primary" /> Vacancy Opportunity (Optional)
          </label>
          <CustomSelect
            value={vacancyId}
            onChange={(val) => setVacancyId(val)}
            disabled={!companyId || isLoadingVacancies}
            placeholder="No specific vacancy (General Application)"
            options={[
              { value: '', label: 'No specific vacancy (General Application)' },
              ...vacancies.map((vacancy) => ({
                value: vacancy.id,
                label: vacancy.title,
              })),
            ]}
            className="w-full"
          />
        </div>

        {/* Resume Revision Select */}
        <div>
          <label className="block text-xs font-semibold text-foreground mb-1.5 flex items-center gap-1.5">
            <FileText className="h-3.5 w-3.5 text-primary" /> Resume Revision <span className="text-destructive">*</span>
          </label>
          <CustomSelect
            value={resumeRevisionId}
            onChange={(val) => setResumeRevisionId(val)}
            disabled={isLoadingResumes}
            placeholder="Select a resume revision..."
            options={resumeRevisions.map((rev) => ({
              value: rev.id,
              label: `v${rev.version} — ${rev.status} (${new Date(rev.createdAt).toLocaleDateString()})`,
            }))}
            className="w-full"
          />
          {resumeRevisions.length === 0 && !isLoadingResumes && (
            <p className="mt-1 text-xs text-amber-500 flex items-center gap-1">
              <AlertCircle className="h-3 w-3" /> No resume revisions found. Please create a resume draft in Resumes section first.
            </p>
          )}
        </div>

        {/* Job Source */}
        <div>
          <label className="block text-xs font-semibold text-foreground mb-1.5 flex items-center gap-1.5">
            <Globe className="h-3.5 w-3.5 text-primary" /> Job Source <span className="text-destructive">*</span>
          </label>
          <div className="flex flex-wrap gap-2 mb-2">
            {COMMON_SOURCES.map((source) => (
              <button
                key={source}
                type="button"
                onClick={() => setJobSource(source)}
                className={`px-2.5 py-1 text-xs rounded-lg border font-medium transition-colors ${
                  jobSource === source
                    ? 'bg-primary/20 text-primary border-primary'
                    : 'bg-background text-muted-foreground border-border hover:bg-accent'
                }`}
              >
                {source}
              </button>
            ))}
            <button
              type="button"
              onClick={() => setJobSource('Custom')}
              className={`px-2.5 py-1 text-xs rounded-lg border font-medium transition-colors ${
                jobSource === 'Custom'
                  ? 'bg-primary/20 text-primary border-primary'
                  : 'bg-background text-muted-foreground border-border hover:bg-accent'
              }`}
            >
              Custom Source...
            </button>
          </div>

          {jobSource === 'Custom' && (
            <input
              type="text"
              value={customJobSource}
              onChange={(e) => setCustomJobSource(e.target.value)}
              placeholder="Enter custom job source (e.g., Telegram, Job Fair)..."
              required
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground focus:outline-none focus:ring-2 focus:ring-primary mt-1"
            />
          )}
        </div>

        {/* Notes */}
        <div>
          <label className="block text-xs font-semibold text-foreground mb-1.5">
            Application Notes
          </label>
          <textarea
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
            rows={3}
            placeholder="Add initial notes, recruiter contact info, cover letter link..."
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        {/* Submit Immediately Toggle */}
        <div className="rounded-xl border border-border/60 bg-accent/30 p-3.5">
          <label className="flex items-center gap-3 cursor-pointer">
            <input
              type="checkbox"
              checked={submitImmediately}
              onChange={(e) => setSubmitImmediately(e.target.checked)}
              className="h-4 w-4 rounded border-border text-primary focus:ring-primary accent-primary"
            />
            <div>
              <span className="text-xs font-bold text-foreground block">
                Submit Immediately (Status = Applied)
              </span>
              <span className="text-[11px] text-muted-foreground block">
                If checked, sets status to &quot;Applied&quot; and locks linked Resume Revision per ADR 005.
                If unchecked, saves application as &quot;Draft&quot;.
              </span>
            </div>
          </label>
        </div>

        {/* Footer actions */}
        <div className="flex items-center justify-end gap-3 pt-3 border-t border-border/40">
          <button
            type="button"
            onClick={onClose}
            className="rounded-xl px-4 py-2 text-xs font-semibold text-muted-foreground hover:bg-accent hover:text-foreground transition-colors"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={!isFormValid || submitApplicationMutation.isPending}
            className="inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all disabled:opacity-50"
          >
            {submitApplicationMutation.isPending ? (
              'Submitting...'
            ) : (
              <>
                <Send className="h-3.5 w-3.5" /> Submit Application
              </>
            )}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default ApplicationFormModal;
