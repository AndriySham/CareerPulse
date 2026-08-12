import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useVacancy } from '@/api/vacancies';
import { useCompany } from '@/api/companies';
import { useResumeRevisions, resumesKeys } from '@/api/resumes';
import { useApplications, useSubmitApplication } from '@/api/applications';
import { useQueryClient } from '@tanstack/react-query';
import ResumeRevisionSelect from '@/components/resumes/ResumeRevisionSelect';
import VacancyFormModal from '@/components/vacancies/VacancyFormModal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import ApplicationStatusBadge from '@/components/applications/ApplicationStatusBadge';
import type { SubmitApplicationDto, ApplicationDto } from '@/types';
import {
  ArrowLeft,
  Briefcase,
  Building2,
  Calendar,
  ExternalLink,
  Edit3,
  Clock,
  Send,
  CheckCircle2,
  Globe,
  FileText,
  AlertCircle,
  Tag,
  ChevronRight,
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

export const VacancyDetailPage: React.FC = () => {
  const { vacancyId } = useParams<{ vacancyId: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  // Modal State for Editing Vacancy
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  // Queries
  const {
    data: vacancy,
    isLoading: isLoadingVacancy,
    isError: isErrorVacancy,
    error: vacancyError,
    refetch: refetchVacancy,
  } = useVacancy(vacancyId);

  const { data: company } = useCompany(vacancy?.companyId);
  const { data: resumeRevisions = [], isLoading: isLoadingResumes } = useResumeRevisions();
  const { data: existingApplications = [] } = useApplications(
    vacancy ? { vacancyId: vacancy.id } : undefined
  );

  // Form & Submission State
  const [selectedRevisionId, setSelectedRevisionId] = useState('');
  const [jobSource, setJobSource] = useState('LinkedIn');
  const [customJobSource, setCustomJobSource] = useState('');
  const [notes, setNotes] = useState('');
  const [submissionSuccess, setSubmissionSuccess] = useState(false);
  const [submittedApp, setSubmittedApp] = useState<ApplicationDto | null>(null);

  const submitApplicationMutation = useSubmitApplication();

  // Set default selected revision once revisions are loaded
  useEffect(() => {
    if (resumeRevisions.length > 0 && !selectedRevisionId) {
      setSelectedRevisionId(resumeRevisions[0].id);
    }
  }, [resumeRevisions, selectedRevisionId]);

  const handleSubmitApplication = (e: React.FormEvent) => {
    e.preventDefault();

    const effectiveSource = jobSource === 'Custom' ? customJobSource.trim() : jobSource;

    if (!vacancy) return;
    if (!selectedRevisionId) return;
    if (!effectiveSource) return;

    const dto: SubmitApplicationDto = {
      companyId: vacancy.companyId,
      vacancyId: vacancy.id,
      resumeRevisionId: selectedRevisionId,
      jobSource: effectiveSource,
      notes: notes.trim() || null,
      submitImmediately: true,
    };

    submitApplicationMutation.mutate(dto, {
      onSuccess: (newApp) => {
        queryClient.invalidateQueries({ queryKey: resumesKeys.all });
        setSubmissionSuccess(true);
        setSubmittedApp(newApp);
      },
    });
  };

  const isFormValid = Boolean(
    selectedRevisionId && (jobSource !== 'Custom' || customJobSource.trim().length > 0)
  );

  // Loading State
  if (isLoadingVacancy) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-3">
          <div className="h-8 w-24 rounded-lg bg-accent/60 animate-pulse" />
        </div>
        <div className="h-28 rounded-xl border border-border/60 bg-card p-6 animate-pulse space-y-4">
          <div className="h-7 w-1/3 bg-accent/60 rounded" />
          <div className="h-4 w-1/4 bg-accent/40 rounded" />
        </div>
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div className="h-80 rounded-xl border border-border/60 bg-card p-6 animate-pulse" />
          <div className="h-80 rounded-xl border border-border/60 bg-card p-6 animate-pulse" />
        </div>
      </div>
    );
  }

  // Error State
  if (isErrorVacancy) {
    return (
      <div className="space-y-6">
        <button
          type="button"
          onClick={() => navigate('/vacancies')}
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors"
        >
          <ArrowLeft className="h-4 w-4" /> Back to Vacancies
        </button>
        <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
          <AlertCircle className="mx-auto h-10 w-10 text-destructive mb-3" />
          <h3 className="text-base font-semibold text-destructive">Failed to load vacancy details</h3>
          <p className="text-xs text-muted-foreground mt-1 mb-4">
            {vacancyError instanceof Error ? vacancyError.message : 'An error occurred while fetching the vacancy.'}
          </p>
          <button
            type="button"
            onClick={() => refetchVacancy()}
            className="rounded-lg bg-destructive px-4 py-2 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90 transition-colors"
          >
            Retry Loading
          </button>
        </div>
      </div>
    );
  }

  // Vacancy Not Found State
  if (!vacancy) {
    return (
      <div className="space-y-6">
        <button
          type="button"
          onClick={() => navigate('/vacancies')}
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors"
        >
          <ArrowLeft className="h-4 w-4" /> Back to Vacancies
        </button>
        <div className="rounded-xl border border-dashed border-border p-12 text-center shadow-sm">
          <Briefcase className="mx-auto h-12 w-12 text-muted-foreground/40 mb-3" />
          <h3 className="text-base font-semibold text-foreground">Vacancy Not Found</h3>
          <p className="mt-1 text-sm text-muted-foreground max-w-sm mx-auto">
            The requested vacancy may have been removed or does not exist.
          </p>
          <Link
            to="/vacancies"
            className="mt-4 inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all"
          >
            <ArrowLeft className="h-4 w-4" /> Return to Vacancies Directory
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6 pb-12">
      {/* Top Navigation & Breadcrumbs */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2 text-xs text-muted-foreground">
          <Link to="/vacancies" className="flex items-center gap-1 hover:text-foreground transition-colors font-medium">
            <ArrowLeft className="h-3.5 w-3.5" /> Vacancies
          </Link>
          <ChevronRight className="h-3 w-3 text-muted-foreground/50" />
          {company && (
            <>
              <span className="truncate max-w-[150px]">{company.name}</span>
              <ChevronRight className="h-3 w-3 text-muted-foreground/50" />
            </>
          )}
          <span className="font-semibold text-foreground truncate max-w-[200px]">{vacancy.title}</span>
        </div>

        <button
          type="button"
          onClick={() => setIsEditModalOpen(true)}
          className="inline-flex items-center gap-1.5 rounded-lg border border-border bg-card px-3 py-1.5 text-xs font-semibold text-foreground hover:bg-accent transition-colors cursor-pointer"
        >
          <Edit3 className="h-3.5 w-3.5 text-primary" /> Edit Vacancy
        </button>
      </div>

      {/* Hero / Header Card */}
      <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
        <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
          <div className="flex items-start gap-4">
            <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary font-bold">
              <Briefcase className="h-6 w-6" />
            </div>
            <div className="space-y-1">
              <h1 className="text-2xl font-bold tracking-tight text-foreground">{vacancy.title}</h1>
              <div className="flex flex-wrap items-center gap-3 text-xs pt-0.5">
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

                <span className="inline-flex items-center gap-1.5 text-muted-foreground text-xs">
                  <Clock className="h-3.5 w-3.5" />
                  Added {new Date(vacancy.createdAt).toLocaleDateString()}
                </span>
              </div>
            </div>
          </div>

          {/* External Original Listing Link */}
          {vacancy.url && (
            <a
              href={vacancy.url}
              target="_blank"
              rel="noopener noreferrer"
              className="inline-flex items-center justify-center gap-2 rounded-xl border border-border bg-accent/50 px-4 py-2 text-xs font-semibold text-foreground hover:bg-accent hover:text-primary transition-all shrink-0"
            >
              <span>View Original Listing</span>
              <ExternalLink className="h-3.5 w-3.5" />
            </a>
          )}
        </div>
      </div>

      {/* Main Responsive Grid Layout (Balanced 50% / 50% split on desktop) */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Left Column (50%): Vacancy Details, Description & Company Info */}
        <div className="space-y-6">
          {/* Existing Application Notice */}
          {existingApplications.length > 0 && !submissionSuccess && (
            <div className="rounded-xl border border-primary/30 bg-primary/10 p-4 space-y-2">
              <div className="flex items-center justify-between flex-wrap gap-2">
                <div className="flex items-center gap-2 text-xs font-bold text-primary">
                  <CheckCircle2 className="h-4 w-4 shrink-0" />
                  Existing Application Tracked ({existingApplications.length})
                </div>
                <ApplicationStatusBadge status={existingApplications[0].status} />
              </div>
              <p className="text-xs text-muted-foreground">
                Source: <strong className="text-foreground">{existingApplications[0].jobSource}</strong> •
                Submitted: {new Date(existingApplications[0].createdAt).toLocaleDateString()}
                {existingApplications[0].notes && (
                  <span className="block mt-1 italic text-muted-foreground/90">
                    "{existingApplications[0].notes}"
                  </span>
                )}
              </p>
            </div>
          )}

          {/* Job Description Card */}
          <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
            <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
              <FileText className="h-4 w-4 text-primary" />
              Job Description & Requirements
            </h2>

            {vacancy.description ? (
              <div className="text-sm text-foreground whitespace-pre-wrap leading-relaxed">
                {vacancy.description}
              </div>
            ) : (
              <div className="rounded-xl border border-dashed border-border p-8 text-center">
                <p className="text-xs text-muted-foreground italic">No detailed description attached to this vacancy.</p>
              </div>
            )}
          </div>

          {/* Employer Snapshot Card */}
          {company && (
            <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
              <div className="flex items-center justify-between border-b border-border/40 pb-3">
                <h3 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2">
                  <Building2 className="h-4 w-4 text-primary" />
                  About Employer
                </h3>
                {company.website && (
                  <a
                    href={company.website}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="inline-flex items-center gap-1 text-xs font-medium text-primary hover:underline"
                  >
                    <Globe className="h-3.5 w-3.5" />
                    Website <ExternalLink className="h-3 w-3" />
                  </a>
                )}
              </div>

              <div className="space-y-3 text-xs">
                <div className="flex items-center gap-3">
                  <span className="font-bold text-foreground text-sm">{company.name}</span>
                  {company.industry && (
                    <span className="inline-flex items-center gap-1 rounded-full bg-primary/10 px-2.5 py-0.5 font-medium text-primary border border-primary/20">
                      <Tag className="h-3 w-3" /> {company.industry}
                    </span>
                  )}
                </div>

                {company.notes && (
                  <div className="rounded-lg bg-muted/30 p-3 text-muted-foreground border border-border/40 leading-relaxed">
                    <strong className="text-foreground block mb-0.5">Internal Employer Notes:</strong>
                    {company.notes}
                  </div>
                )}
              </div>
            </div>
          )}
        </div>

        {/* Right Column (1/3): Application Submission Sidebar Workflow */}
        <div className="space-y-6">
          <div className="sticky top-6 rounded-xl border border-border/80 bg-card p-5 space-y-4 shadow-md">
            <div className="flex items-center justify-between border-b border-border/40 pb-3">
              <h3 className="text-base font-bold tracking-tight text-foreground flex items-center gap-2">
                <Send className="h-4.5 w-4.5 text-primary" /> Apply for Vacancy
              </h3>
              <span className="text-[11px] font-semibold text-primary bg-primary/10 px-2 py-0.5 rounded-full border border-primary/20">
                CRM Workflow
              </span>
            </div>

            {/* Submission Success Banner */}
            {submissionSuccess ? (
              <div className="rounded-xl border border-emerald-500/30 bg-emerald-500/10 p-4 text-emerald-400 space-y-3">
                <div className="flex items-center gap-2 font-semibold text-sm">
                  <CheckCircle2 className="h-5 w-5 text-emerald-400 shrink-0" />
                  <span>Application Registered!</span>
                </div>
                <p className="text-xs text-emerald-300/90 leading-relaxed">
                  Your application for <strong className="text-emerald-200">{vacancy.title}</strong> at{' '}
                  <strong className="text-emerald-200">{company?.name || 'Company'}</strong> has been saved with status{' '}
                  <strong className="text-emerald-200">Applied</strong>.
                </p>
                {submittedApp && (
                  <div className="pt-2 border-t border-emerald-500/20 text-xs text-emerald-300/90 space-y-1">
                    <div>Job Source: <strong>{submittedApp.jobSource}</strong></div>
                    <div>Submitted: <strong>{new Date(submittedApp.createdAt).toLocaleTimeString()}</strong></div>
                  </div>
                )}
                <div className="pt-2">
                  <button
                    type="button"
                    onClick={() => setSubmissionSuccess(false)}
                    className="text-xs font-semibold text-emerald-400 underline hover:text-emerald-300 cursor-pointer"
                  >
                    Submit another revision or edit notes
                  </button>
                </div>
              </div>
            ) : (
              <form onSubmit={handleSubmitApplication} className="space-y-4">
                {submitApplicationMutation.isError && (
                  <ErrorAlert error={submitApplicationMutation.error} />
                )}

                {/* Resume Revision Selector */}
                <div>
                  <label className="block text-xs font-semibold text-foreground mb-1.5 flex items-center gap-1.5">
                    <FileText className="h-3.5 w-3.5 text-primary" /> Resume Revision <span className="text-destructive">*</span>
                  </label>
                  <ResumeRevisionSelect
                    value={selectedRevisionId}
                    onChange={(id) => setSelectedRevisionId(id)}
                    revisions={resumeRevisions}
                    isLoading={isLoadingResumes}
                    disabled={submitApplicationMutation.isPending}
                  />
                  {resumeRevisions.length === 0 && !isLoadingResumes && (
                    <p className="mt-1.5 text-xs text-amber-500 flex items-center gap-1">
                      <AlertCircle className="h-3.5 w-3.5 shrink-0" /> No resume revisions available. Please create a draft resume first.
                    </p>
                  )}
                </div>

                {/* Job Source */}
                <div>
                  <label className="block text-xs font-semibold text-foreground mb-1.5 flex items-center gap-1.5">
                    <Globe className="h-3.5 w-3.5 text-primary" /> Job Source <span className="text-destructive">*</span>
                  </label>
                  <div className="flex flex-wrap gap-1.5 mb-2">
                    {COMMON_SOURCES.map((source) => (
                      <button
                        key={source}
                        type="button"
                        onClick={() => setJobSource(source)}
                        className={`px-2.5 py-1 text-xs rounded-lg border font-medium transition-colors cursor-pointer ${
                          jobSource === source
                            ? 'bg-primary/20 text-primary border-primary font-semibold'
                            : 'bg-background text-muted-foreground border-border hover:bg-accent hover:text-foreground'
                        }`}
                      >
                        {source}
                      </button>
                    ))}
                    <button
                      type="button"
                      onClick={() => setJobSource('Custom')}
                      className={`px-2.5 py-1 text-xs rounded-lg border font-medium transition-colors cursor-pointer ${
                        jobSource === 'Custom'
                          ? 'bg-primary/20 text-primary border-primary font-semibold'
                          : 'bg-background text-muted-foreground border-border hover:bg-accent hover:text-foreground'
                      }`}
                    >
                      Custom...
                    </button>
                  </div>

                  {jobSource === 'Custom' && (
                    <input
                      type="text"
                      value={customJobSource}
                      onChange={(e) => setCustomJobSource(e.target.value)}
                      placeholder="Enter custom source (e.g., Telegram, Referral)..."
                      required
                      className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground focus:outline-none focus:ring-2 focus:ring-primary mt-1"
                    />
                  )}
                </div>

                {/* Application Notes */}
                <div>
                  <label className="block text-xs font-semibold text-foreground mb-1.5">
                    Application Notes (Optional)
                  </label>
                  <textarea
                    value={notes}
                    onChange={(e) => setNotes(e.target.value)}
                    rows={3}
                    placeholder="Add optional notes, cover letter info, recruiter details..."
                    className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs text-foreground placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
                  />
                </div>

                {/* Submit Action Button */}
                <button
                  type="submit"
                  disabled={!isFormValid || submitApplicationMutation.isPending || resumeRevisions.length === 0}
                  className="w-full inline-flex items-center justify-center gap-2 rounded-xl bg-primary px-4 py-3 text-xs font-bold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {submitApplicationMutation.isPending ? (
                    'Submitting Application...'
                  ) : (
                    <>
                      <Send className="h-4 w-4" /> Submit Application
                    </>
                  )}
                </button>
              </form>
            )}
          </div>
        </div>
      </div>

      {/* Edit Vacancy Modal */}
      <VacancyFormModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        vacancyToEdit={vacancy}
      />
    </div>
  );
};

export default VacancyDetailPage;
