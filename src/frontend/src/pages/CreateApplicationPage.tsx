import React, { useState, useEffect } from 'react';
import { useParams, useSearchParams, useNavigate, Link } from 'react-router-dom';
import { useVacancy } from '@/api/vacancies';
import { useCompany, useCompanies } from '@/api/companies';
import { useResumeRevisions } from '@/api/resumes';
import { useSubmitApplication } from '@/api/applications';
import ResumeRevisionSelect from '@/components/resumes/ResumeRevisionSelect';
import CustomSelect from '@/components/ui/CustomSelect';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { formatSalary, formatWorkMode } from '@/types';
import type { SubmitApplicationDto } from '@/types';
import {
  ArrowLeft,
  Briefcase,
  Building2,
  FileText,
  Globe,
  Send,
  MapPin,
  Laptop,
  Banknote,
  ExternalLink,
  AlertCircle,
} from 'lucide-react';

const COMMON_SOURCES = [
  'LinkedIn',
  'Djinni',
  'DOU',
  'Company Site',
  'Referral',
  'Glassdoor',
  'Indeed',
];

export const CreateApplicationPage: React.FC = () => {
  const { vacancyId: routeVacancyId } = useParams<{ vacancyId?: string }>();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  const queryVacancyId = searchParams.get('vacancyId') ?? '';
  const queryCompanyId = searchParams.get('companyId') ?? '';

  const effectiveVacancyId = routeVacancyId || queryVacancyId;

  // Data fetching
  const {
    data: vacancy,
    isLoading: isLoadingVacancy,
  } = useVacancy(effectiveVacancyId ? effectiveVacancyId : undefined);

  const effectiveCompanyId = vacancy?.companyId || queryCompanyId;
  const { data: company } = useCompany(
    effectiveCompanyId ? effectiveCompanyId : undefined
  );

  const { data: companies = [], isLoading: isLoadingCompanies } = useCompanies();
  const { data: resumeRevisions = [], isLoading: isLoadingResumes } = useResumeRevisions();

  // Form State
  const [selectedCompanyId, setSelectedCompanyId] = useState(effectiveCompanyId);
  const [selectedRevisionId, setSelectedRevisionId] = useState('');
  const [jobSource, setJobSource] = useState('LinkedIn');
  const [customJobSource, setCustomJobSource] = useState('');
  const [notes, setNotes] = useState('');
  const [submitImmediately, setSubmitImmediately] = useState(true);
  const [validationError, setValidationError] = useState<string | null>(null);

  const submitMutation = useSubmitApplication();

  // Synchronize companyId from vacancy if loaded
  useEffect(() => {
    if (vacancy?.companyId) {
      setSelectedCompanyId(vacancy.companyId);
    } else if (queryCompanyId) {
      setSelectedCompanyId(queryCompanyId);
    }
  }, [vacancy, queryCompanyId]);

  // Set default selected revision
  useEffect(() => {
    if (resumeRevisions.length > 0 && !selectedRevisionId) {
      setSelectedRevisionId(resumeRevisions[0].id);
    }
  }, [resumeRevisions, selectedRevisionId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);

    const activeCompanyId = vacancy?.companyId || selectedCompanyId;
    if (!activeCompanyId) {
      setValidationError('Please select a target company for this application.');
      return;
    }

    if (!selectedRevisionId) {
      setValidationError('Please select a resume revision to submit.');
      return;
    }

    const effectiveSource = jobSource === 'Custom' ? customJobSource.trim() : jobSource;
    if (!effectiveSource) {
      setValidationError('Please specify the application source.');
      return;
    }

    const dto: SubmitApplicationDto = {
      companyId: activeCompanyId,
      vacancyId: effectiveVacancyId || null,
      resumeRevisionId: selectedRevisionId,
      jobSource: effectiveSource,
      notes: notes.trim() || null,
      submitImmediately,
    };

    try {
      await submitMutation.mutateAsync(dto);
      // After submission, return to Vacancy Details page if linked, otherwise Applications list
      if (effectiveVacancyId) {
        navigate(`/vacancies/${effectiveVacancyId}`);
      } else {
        navigate('/applications');
      }
    } catch {
      // Handled by submitMutation.error
    }
  };

  const backUrl = effectiveVacancyId ? `/vacancies/${effectiveVacancyId}` : '/applications';
  const salaryText = vacancy ? formatSalary(vacancy.salaryMin, vacancy.salaryMax, vacancy.salaryCurrency) : null;
  const workModeText = vacancy ? formatWorkMode(vacancy.workMode) : null;

  return (
    <div className="space-y-6 max-w-4xl mx-auto pb-16">
      {/* Top Back Navigation */}
      <div className="flex items-center justify-between border-b border-border/40 pb-4">
        <Link
          to={backUrl}
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors"
        >
          <ArrowLeft className="h-4 w-4" />
          {effectiveVacancyId ? 'Back to Vacancy Details' : 'Back to Applications'}
        </Link>
      </div>

      {/* Page Header */}
      <div className="flex items-start gap-4">
        <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary font-bold">
          <Send className="h-6 w-6" />
        </div>
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-foreground">
            {vacancy ? `Apply for ${vacancy.title}` : 'Submit Job Application'}
          </h1>
          <p className="text-sm text-muted-foreground">
            Connect your tailored resume revision and register this application into your CRM pipeline.
          </p>
        </div>
      </div>

      {/* Vacancy Context Banner (when applying for a specific vacancy) */}
      {effectiveVacancyId && (
        <div className="rounded-xl border border-primary/30 bg-primary/5 p-5 shadow-sm space-y-3">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
            <div className="space-y-1">
              <span className="text-[11px] font-bold uppercase tracking-wider text-primary">
                Targeted Opportunity
              </span>
              <h2 className="text-lg font-bold text-foreground flex items-center gap-2">
                <Briefcase className="h-5 w-5 text-primary shrink-0" />
                {vacancy?.title || (isLoadingVacancy ? 'Loading vacancy...' : 'Job Vacancy')}
              </h2>
              {company && (
                <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
                  <Building2 className="h-3.5 w-3.5 text-primary/70" />
                  <span className="font-semibold text-foreground">{company.name}</span>
                  {company.industry && <span>({company.industry})</span>}
                </div>
              )}
            </div>

            {vacancy?.url && (
              <a
                href={vacancy.url}
                target="_blank"
                rel="noopener noreferrer"
                className="inline-flex items-center gap-1.5 rounded-lg border border-border bg-card px-3 py-1.5 text-xs font-medium text-foreground hover:text-primary transition-colors shrink-0"
              >
                <span>Original Posting</span>
                <ExternalLink className="h-3.5 w-3.5" />
              </a>
            )}
          </div>

          {/* Quick vacancy tags */}
          {vacancy && (
            <div className="flex flex-wrap items-center gap-2 pt-1 text-xs">
              {workModeText && (
                <span className="inline-flex items-center gap-1 rounded-md bg-accent/60 px-2.5 py-1 font-medium text-foreground">
                  <Laptop className="h-3.5 w-3.5 text-primary" />
                  {workModeText}
                </span>
              )}
              {vacancy.location && (
                <span className="inline-flex items-center gap-1 rounded-md bg-accent/60 px-2.5 py-1 font-medium text-muted-foreground">
                  <MapPin className="h-3.5 w-3.5 text-primary" />
                  {vacancy.location}
                </span>
              )}
              {salaryText && (
                <span className="inline-flex items-center gap-1 rounded-md bg-emerald-500/10 px-2.5 py-1 font-semibold text-emerald-400 border border-emerald-500/20">
                  <Banknote className="h-3.5 w-3.5" />
                  {salaryText}
                </span>
              )}
            </div>
          )}
        </div>
      )}

      <form onSubmit={handleSubmit} autoComplete="off" className="space-y-6">
        {/* Error Alerts */}
        {validationError && (
          <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-4 text-xs font-semibold text-destructive flex items-center gap-2">
            <AlertCircle className="h-4 w-4 shrink-0" />
            <span>{validationError}</span>
          </div>
        )}
        {submitMutation.error && <ErrorAlert error={submitMutation.error} />}

        {/* Company Selector (if not pre-selected by vacancy) */}
        {!effectiveVacancyId && (
          <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
            <h3 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
              <Building2 className="h-4 w-4 text-primary" /> Target Employer <span className="text-destructive">*</span>
            </h3>
            {isLoadingCompanies ? (
              <div className="h-10 w-full rounded-lg bg-accent/40 animate-pulse" />
            ) : companies.length === 0 ? (
              <p className="text-xs text-destructive">
                No active companies found. Please create a company in the Companies section first.
              </p>
            ) : (
              <CustomSelect
                value={selectedCompanyId}
                onChange={(val) => setSelectedCompanyId(val)}
                placeholder="Select a company..."
                options={companies.map((c) => ({
                  value: c.id,
                  label: `${c.name}${c.industry ? ` (${c.industry})` : ''}`,
                }))}
                className="w-full"
              />
            )}
          </div>
        )}

        {/* Section 1: Resume Revision Selector */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <h3 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
            <FileText className="h-4 w-4 text-primary" /> Linked Resume Revision <span className="text-destructive">*</span>
          </h3>

          <ResumeRevisionSelect
            value={selectedRevisionId}
            onChange={(id) => setSelectedRevisionId(id)}
            revisions={resumeRevisions}
            isLoading={isLoadingResumes}
            disabled={submitMutation.isPending}
            showDetailsCard={true}
          />

          {resumeRevisions.length === 0 && !isLoadingResumes && (
            <div className="rounded-lg border border-amber-500/30 bg-amber-500/10 p-3 text-xs text-amber-400 flex items-center gap-2">
              <AlertCircle className="h-4 w-4 shrink-0" />
              <span>
                No resume drafts available. Please create a resume revision in the{' '}
                <Link to="/resumes/new" className="underline font-semibold hover:text-amber-300">
                  Resumes
                </Link>{' '}
                section before applying.
              </span>
            </div>
          )}
        </div>

        {/* Section 2: Application Source & Channel */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <h3 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
            <Globe className="h-4 w-4 text-primary" /> Application Channel / Source <span className="text-destructive">*</span>
          </h3>

          <div>
            <label className="block text-xs font-semibold text-muted-foreground mb-2">
              Select where you found or submitted this opening:
            </label>
            <div className="flex flex-wrap gap-2 mb-3">
              {COMMON_SOURCES.map((source) => (
                <button
                  key={source}
                  type="button"
                  onClick={() => setJobSource(source)}
                  className={`px-3.5 py-1.5 text-xs rounded-xl border font-semibold transition-all cursor-pointer ${
                    jobSource === source
                      ? 'bg-primary text-primary-foreground border-primary shadow-sm'
                      : 'bg-background text-muted-foreground border-border hover:bg-accent hover:text-foreground'
                  }`}
                >
                  {source}
                </button>
              ))}
              <button
                type="button"
                onClick={() => setJobSource('Custom')}
                className={`px-3.5 py-1.5 text-xs rounded-xl border font-semibold transition-all cursor-pointer ${
                  jobSource === 'Custom'
                    ? 'bg-primary text-primary-foreground border-primary shadow-sm'
                    : 'bg-background text-muted-foreground border-border hover:bg-accent hover:text-foreground'
                }`}
              >
                Custom Source...
              </button>
            </div>

            {jobSource === 'Custom' && (
              <div className="mt-2">
                <input
                  type="text"
                  value={customJobSource}
                  onChange={(e) => setCustomJobSource(e.target.value)}
                  placeholder="Enter custom source name (e.g., Telegram channel, Recruiter reach-out)..."
                  className="w-full rounded-lg border border-border bg-background px-3.5 py-2 text-sm text-foreground placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
                />
              </div>
            )}
          </div>
        </div>

        {/* Section 3: Notes & Additional Correspondence */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <h3 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
            <FileText className="h-4 w-4 text-primary" /> Application Notes & Initial Context
          </h3>

          <div>
            <textarea
              rows={4}
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              placeholder="Add recruiter contact, cover letter link, initial follow-up dates, or custom context..."
              className="w-full rounded-lg border border-border bg-background px-3.5 py-3 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary leading-relaxed resize-y"
            />
          </div>

          {/* Submit Immediately Toggle */}
          <div className="rounded-xl border border-border/60 bg-accent/30 p-4">
            <label className="flex items-start gap-3 cursor-pointer">
              <input
                type="checkbox"
                checked={submitImmediately}
                onChange={(e) => setSubmitImmediately(e.target.checked)}
                className="h-4 w-4 mt-0.5 rounded border-border text-primary focus:ring-primary accent-primary"
              />
              <div>
                <span className="text-xs font-bold text-foreground block">
                  Submit Immediately (Status = Applied)
                </span>
                <span className="text-[11px] text-muted-foreground block mt-0.5">
                  If checked, registers this application as &quot;Applied&quot; and locks the linked Resume Revision (ADR 005 Immutability).
                  If unchecked, saves as &quot;Draft&quot; in your pipeline.
                </span>
              </div>
            </label>
          </div>
        </div>

        {/* Footer Actions */}
        <div className="flex items-center justify-between pt-4 border-t border-border/40">
          <button
            type="button"
            onClick={() => navigate(backUrl)}
            disabled={submitMutation.isPending}
            className="rounded-xl px-5 py-2.5 text-sm font-semibold text-muted-foreground hover:bg-accent hover:text-foreground transition-colors disabled:opacity-50 cursor-pointer"
          >
            Cancel
          </button>

          <button
            type="submit"
            disabled={
              submitMutation.isPending ||
              !selectedRevisionId ||
              resumeRevisions.length === 0 ||
              (!vacancy?.companyId && !selectedCompanyId)
            }
            className="inline-flex items-center gap-2 rounded-xl bg-primary px-6 py-2.5 text-sm font-bold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all disabled:opacity-50 cursor-pointer"
          >
            {submitMutation.isPending ? (
              'Submitting Application...'
            ) : (
              <>
                <Send className="h-4 w-4" />
                Submit Application
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  );
};

export default CreateApplicationPage;
