import React, { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useVacancy } from '@/api/vacancies';
import { useCompany } from '@/api/companies';
import { useApplications } from '@/api/applications';
import { useResumeRevision } from '@/api/resumes';
import ApplicationStatusBadge from '@/components/applications/ApplicationStatusBadge';
import ApplicationDetailModal from '@/components/applications/ApplicationDetailModal';
import { formatSalary, formatWorkMode, formatEmploymentType } from '@/types';
import type { ApplicationDto } from '@/types';
import {
  ArrowLeft,
  Briefcase,
  Building2,
  Calendar,
  ExternalLink,
  Edit3,
  Clock,
  Send,
  Globe,
  FileText,
  AlertCircle,
  Tag,
  ChevronRight,
  MapPin,
  Laptop,
  Banknote,
  ArrowRight,
  ListChecks,
  CheckCircle2,
  Sparkles,
  Gift,
} from 'lucide-react';

export const VacancyDetailPage: React.FC = () => {
  const { vacancyId } = useParams<{ vacancyId: string }>();
  const navigate = useNavigate();

  // Queries
  const {
    data: vacancy,
    isLoading: isLoadingVacancy,
    isError: isErrorVacancy,
    error: vacancyError,
    refetch: refetchVacancy,
  } = useVacancy(vacancyId);

  const { data: company } = useCompany(vacancy?.companyId);

  // Check if an application exists for this vacancy
  const { data: existingApplications = [] } = useApplications(
    vacancy ? { vacancyId: vacancy.id } : undefined
  );

  // Application detail modal state
  const [selectedApplication, setSelectedApplication] = useState<ApplicationDto | null>(null);
  const [isDetailModalOpen, setIsDetailModalOpen] = useState(false);

  const primaryApplication = existingApplications.length > 0 ? existingApplications[0] : null;

  // Load linked resume info if an application exists
  const { data: linkedResume } = useResumeRevision(primaryApplication?.resumeRevisionId);

  // Handlers
  const handleApplyClick = () => {
    if (!vacancy) return;
    navigate(`/applications/new?vacancyId=${vacancy.id}&companyId=${vacancy.companyId}`);
  };

  const handleEditClick = () => {
    if (!vacancy) return;
    navigate(`/vacancies/${vacancy.id}/edit`);
  };

  const handleOpenApplicationModal = (app: ApplicationDto) => {
    setSelectedApplication(app);
    setIsDetailModalOpen(true);
  };

  // Loading State
  if (isLoadingVacancy) {
    return (
      <div className="space-y-6 max-w-6xl mx-auto py-4">
        <div className="flex items-center gap-3">
          <div className="h-8 w-24 rounded-lg bg-accent/60 animate-pulse" />
        </div>
        <div className="h-36 rounded-xl border border-border/60 bg-card p-6 animate-pulse space-y-4">
          <div className="h-7 w-1/3 bg-accent/60 rounded" />
          <div className="h-4 w-1/4 bg-accent/40 rounded" />
        </div>
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <div className="lg:col-span-2 h-96 rounded-xl border border-border/60 bg-card p-6 animate-pulse" />
          <div className="h-80 rounded-xl border border-border/60 bg-card p-6 animate-pulse" />
        </div>
      </div>
    );
  }

  // Error State
  if (isErrorVacancy) {
    return (
      <div className="space-y-6 max-w-6xl mx-auto py-4">
        <button
          type="button"
          onClick={() => navigate('/vacancies')}
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors cursor-pointer"
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
      <div className="space-y-6 max-w-6xl mx-auto py-4">
        <button
          type="button"
          onClick={() => navigate('/vacancies')}
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors cursor-pointer"
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

  const salaryFormatted = formatSalary(vacancy.salaryMin, vacancy.salaryMax, vacancy.salaryCurrency);
  const workModeFormatted = formatWorkMode(vacancy.workMode);
  const employmentTypeFormatted = formatEmploymentType(vacancy.employmentType);

  return (
    <div className="space-y-6 max-w-6xl mx-auto pb-16">
      {/* Top Navigation & Breadcrumbs */}
      <div className="flex items-center justify-between border-b border-border/40 pb-4">
        <div className="flex items-center gap-2 text-xs text-muted-foreground">
          <Link to="/vacancies" className="flex items-center gap-1 hover:text-foreground transition-colors font-medium">
            <ArrowLeft className="h-3.5 w-3.5" /> Vacancies
          </Link>
          <ChevronRight className="h-3 w-3 text-muted-foreground/50" />
          {company && (
            <>
              <span className="truncate max-w-[160px]">{company.name}</span>
              <ChevronRight className="h-3 w-3 text-muted-foreground/50" />
            </>
          )}
          <span className="font-semibold text-foreground truncate max-w-[240px]">{vacancy.title}</span>
        </div>

        <button
          type="button"
          onClick={handleEditClick}
          className="inline-flex items-center gap-1.5 rounded-lg border border-border bg-card px-3.5 py-1.5 text-xs font-semibold text-foreground hover:bg-accent hover:text-primary transition-colors cursor-pointer"
        >
          <Edit3 className="h-3.5 w-3.5 text-primary" /> Edit Vacancy
        </button>
      </div>

      {/* Prominent Header Card with Key Information */}
      <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
        <div className="flex flex-col md:flex-row md:items-start md:justify-between gap-4">
          <div className="flex items-start gap-4">
            <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-xl bg-primary/10 text-primary font-bold">
              <Briefcase className="h-7 w-7" />
            </div>
            <div className="space-y-2">
              <h1 className="text-2xl font-extrabold tracking-tight text-foreground sm:text-3xl">
                {vacancy.title}
              </h1>

              {/* Prominent Metadata Badges Row */}
              <div className="flex flex-wrap items-center gap-2.5 text-xs pt-1">
                {company && (
                  <span className="inline-flex items-center gap-1.5 rounded-full bg-primary/10 px-3 py-1 font-semibold text-primary border border-primary/20">
                    <Building2 className="h-3.5 w-3.5" />
                    {company.name}
                  </span>
                )}

                {workModeFormatted && (
                  <span className="inline-flex items-center gap-1.5 rounded-full bg-accent px-3 py-1 font-semibold text-foreground border border-border/60">
                    <Laptop className="h-3.5 w-3.5 text-primary" />
                    {workModeFormatted}
                  </span>
                )}

                {employmentTypeFormatted && (
                  <span className="inline-flex items-center gap-1.5 rounded-full bg-accent px-3 py-1 font-semibold text-foreground border border-border/60">
                    <Briefcase className="h-3.5 w-3.5 text-primary" />
                    {employmentTypeFormatted}
                  </span>
                )}

                {vacancy.location && (
                  <span className="inline-flex items-center gap-1.5 rounded-full bg-accent px-3 py-1 font-medium text-muted-foreground border border-border/60">
                    <MapPin className="h-3.5 w-3.5 text-primary" />
                    {vacancy.location}
                  </span>
                )}

                {salaryFormatted && (
                  <span className="inline-flex items-center gap-1.5 rounded-full bg-emerald-500/10 px-3 py-1 font-bold text-emerald-400 border border-emerald-500/20">
                    <Banknote className="h-3.5 w-3.5" />
                    {salaryFormatted}
                  </span>
                )}

                {vacancy.postedAt && (
                  <span className="inline-flex items-center gap-1.5 rounded-full bg-accent px-3 py-1 font-medium text-muted-foreground border border-border/60">
                    <Calendar className="h-3.5 w-3.5 text-primary" />
                    Posted: {new Date(vacancy.postedAt).toLocaleDateString()}
                  </span>
                )}
              </div>
            </div>
          </div>

          {/* Header Action Buttons */}
          <div className="flex items-center gap-2.5 shrink-0 pt-2 md:pt-0">
            {vacancy.url && (
              <a
                href={vacancy.url}
                target="_blank"
                rel="noopener noreferrer"
                className="inline-flex items-center gap-2 rounded-xl border border-border bg-accent/40 px-3.5 py-2 text-xs font-semibold text-foreground hover:bg-accent hover:text-primary transition-all"
              >
                <span>Original Listing</span>
                <ExternalLink className="h-3.5 w-3.5" />
              </a>
            )}

            {!primaryApplication && (
              <button
                type="button"
                onClick={handleApplyClick}
                className="inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-bold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
              >
                <Send className="h-3.5 w-3.5" />
                Apply for this Vacancy
              </button>
            )}
          </div>
        </div>
      </div>

      {/* Main Responsive Layout: 2 Columns on Desktop */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
        {/* Primary Content Column: Vacancy Details, Description & Company Information */}
        <div className="lg:col-span-8 space-y-6">
          {/* Vacancy Overview & Key Metadata Card */}
          <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
            <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
              <Briefcase className="h-4 w-4 text-primary" /> Key Vacancy Details
            </h2>

            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4 text-xs">
              {/* Location */}
              <div className="rounded-lg bg-accent/30 p-3 border border-border/40 space-y-1">
                <span className="text-muted-foreground flex items-center gap-1.5 font-medium">
                  <MapPin className="h-3.5 w-3.5 text-primary" /> Location
                </span>
                <div className="font-semibold text-foreground text-sm">
                  {vacancy.location || <span className="text-muted-foreground/60 italic font-normal">Not specified</span>}
                </div>
              </div>

              {/* Work Mode */}
              <div className="rounded-lg bg-accent/30 p-3 border border-border/40 space-y-1">
                <span className="text-muted-foreground flex items-center gap-1.5 font-medium">
                  <Laptop className="h-3.5 w-3.5 text-primary" /> Work Mode
                </span>
                <div className="font-semibold text-foreground text-sm">
                  {workModeFormatted || <span className="text-muted-foreground/60 italic font-normal">Flexible</span>}
                </div>
              </div>

              {/* Employment Type */}
              <div className="rounded-lg bg-accent/30 p-3 border border-border/40 space-y-1">
                <span className="text-muted-foreground flex items-center gap-1.5 font-medium">
                  <Briefcase className="h-3.5 w-3.5 text-primary" /> Employment Type
                </span>
                <div className="font-semibold text-foreground text-sm">
                  {employmentTypeFormatted || <span className="text-muted-foreground/60 italic font-normal">Not specified</span>}
                </div>
              </div>

              {/* Compensation */}
              <div className="rounded-lg bg-accent/30 p-3 border border-border/40 space-y-1">
                <span className="text-muted-foreground flex items-center gap-1.5 font-medium">
                  <Banknote className="h-3.5 w-3.5 text-primary" /> Salary / Compensation
                </span>
                <div className="font-semibold text-emerald-400 text-sm">
                  {salaryFormatted || <span className="text-muted-foreground/60 italic font-normal">Not disclosed</span>}
                </div>
              </div>

              {/* Posted Date */}
              <div className="rounded-lg bg-accent/30 p-3 border border-border/40 space-y-1">
                <span className="text-muted-foreground flex items-center gap-1.5 font-medium">
                  <Calendar className="h-3.5 w-3.5 text-primary" /> Date Posted
                </span>
                <div className="font-semibold text-foreground">
                  {vacancy.postedAt ? new Date(vacancy.postedAt).toLocaleDateString() : 'Not recorded'}
                </div>
              </div>

              {/* Created Date */}
              <div className="rounded-lg bg-accent/30 p-3 border border-border/40 space-y-1">
                <span className="text-muted-foreground flex items-center gap-1.5 font-medium">
                  <Clock className="h-3.5 w-3.5 text-primary" /> Added to CRM
                </span>
                <div className="font-semibold text-foreground">
                  {new Date(vacancy.createdAt).toLocaleDateString()}
                </div>
              </div>

              {/* Updated Date */}
              <div className="rounded-lg bg-accent/30 p-3 border border-border/40 space-y-1">
                <span className="text-muted-foreground flex items-center gap-1.5 font-medium">
                  <Clock className="h-3.5 w-3.5 text-primary" /> Last Updated
                </span>
                <div className="font-semibold text-foreground">
                  {new Date(vacancy.updatedAt).toLocaleDateString()}
                </div>
              </div>
            </div>
          </div>

          {/* Job Description Card */}
          {vacancy.description ? (
            <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
              <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
                <FileText className="h-4 w-4 text-primary" /> Job Description & Overview
              </h2>
              <div className="text-sm text-foreground whitespace-pre-wrap leading-relaxed">
                {vacancy.description}
              </div>
            </div>
          ) : !vacancy.responsibilities && !vacancy.requirements && !vacancy.niceToHave && !vacancy.benefits ? (
            <div className="rounded-xl border border-dashed border-border p-8 text-center bg-card">
              <FileText className="mx-auto h-8 w-8 text-muted-foreground/40 mb-2" />
              <p className="text-xs text-muted-foreground italic">No detailed description or specifications attached to this vacancy.</p>
              <button
                type="button"
                onClick={handleEditClick}
                className="mt-3 inline-flex items-center gap-1.5 text-xs font-semibold text-primary hover:underline cursor-pointer"
              >
                <Edit3 className="h-3.5 w-3.5" /> Add Description & Specifications
              </button>
            </div>
          ) : null}

          {/* Key Responsibilities */}
          {vacancy.responsibilities && (
            <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
              <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
                <ListChecks className="h-4 w-4 text-primary" /> Key Responsibilities
              </h2>
              <div className="text-sm text-foreground whitespace-pre-wrap leading-relaxed">
                {vacancy.responsibilities}
              </div>
            </div>
          )}

          {/* Requirements */}
          {vacancy.requirements && (
            <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
              <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
                <CheckCircle2 className="h-4 w-4 text-primary" /> Requirements & Qualifications
              </h2>
              <div className="text-sm text-foreground whitespace-pre-wrap leading-relaxed">
                {vacancy.requirements}
              </div>
            </div>
          )}

          {/* Nice to Have */}
          {vacancy.niceToHave && (
            <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
              <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
                <Sparkles className="h-4 w-4 text-primary" /> Nice to Have
              </h2>
              <div className="text-sm text-foreground whitespace-pre-wrap leading-relaxed">
                {vacancy.niceToHave}
              </div>
            </div>
          )}

          {/* Benefits */}
          {vacancy.benefits && (
            <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
              <h2 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2 border-b border-border/40 pb-3">
                <Gift className="h-4 w-4 text-primary" /> Benefits & Perks
              </h2>
              <div className="text-sm text-foreground whitespace-pre-wrap leading-relaxed">
                {vacancy.benefits}
              </div>
            </div>
          )}

          {/* About Employer Card */}
          {company && (
            <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
              <div className="flex items-center justify-between border-b border-border/40 pb-3">
                <h3 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2">
                  <Building2 className="h-4 w-4 text-primary" /> About Employer
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
                  <span className="font-bold text-foreground text-base">{company.name}</span>
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

        {/* Sidebar Column: Application Status & CRM Workflow */}
        <div className="lg:col-span-4 space-y-6">
          {/* Application State Section */}
          <div className="rounded-xl border border-border/80 bg-card p-5 shadow-sm space-y-4">
            <div className="flex items-center justify-between border-b border-border/40 pb-3">
              <h3 className="text-sm font-bold uppercase tracking-wider text-muted-foreground flex items-center gap-2">
                <Send className="h-4 w-4 text-primary" /> Application Status
              </h3>
              <span className="text-[11px] font-semibold text-primary bg-primary/10 px-2 py-0.5 rounded-full border border-primary/20">
                CRM State
              </span>
            </div>

            {/* Case A: No application exists */}
            {!primaryApplication ? (
              <div className="space-y-4 text-center py-4">
                <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary">
                  <Send className="h-6 w-6" />
                </div>
                <div className="space-y-1">
                  <h4 className="font-bold text-foreground text-sm">No Application Tracked</h4>
                  <p className="text-xs text-muted-foreground leading-relaxed">
                    You haven&apos;t registered an application for this vacancy yet.
                  </p>
                </div>

                <button
                  type="button"
                  onClick={handleApplyClick}
                  className="w-full inline-flex items-center justify-center gap-2 rounded-xl bg-primary px-4 py-2.5 text-xs font-bold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
                >
                  <Send className="h-3.5 w-3.5" />
                  Apply for this Vacancy
                </button>
              </div>
            ) : (
              /* Case B: Application already exists */
              <div className="space-y-4">
                <div className="rounded-xl border border-primary/30 bg-primary/5 p-4 space-y-3">
                  <div className="flex items-center justify-between">
                    <span className="text-xs font-semibold text-muted-foreground">Status:</span>
                    <ApplicationStatusBadge status={primaryApplication.status} />
                  </div>

                  <div className="space-y-2 text-xs border-t border-border/40 pt-3">
                    {/* Source / Channel */}
                    <div className="flex items-center justify-between">
                      <span className="text-muted-foreground">Source / Channel:</span>
                      <strong className="text-foreground">{primaryApplication.jobSource}</strong>
                    </div>

                    {/* Applied Date / Created Date */}
                    <div className="flex items-center justify-between">
                      <span className="text-muted-foreground">
                        {primaryApplication.appliedAt ? 'Submitted:' : 'Created:'}
                      </span>
                      <strong className="text-foreground">
                        {primaryApplication.appliedAt
                          ? new Date(primaryApplication.appliedAt).toLocaleDateString()
                          : new Date(primaryApplication.createdAt).toLocaleDateString()}
                      </strong>
                    </div>

                    {/* Linked Resume Revision */}
                    <div className="flex items-center justify-between">
                      <span className="text-muted-foreground">Linked Resume:</span>
                      {linkedResume ? (
                        <strong className="text-foreground">
                          v{linkedResume.version} ({linkedResume.track})
                        </strong>
                      ) : (
                        <code className="text-primary font-mono text-[11px]">
                          {primaryApplication.resumeRevisionId.slice(0, 8)}...
                        </code>
                      )}
                    </div>
                  </div>

                  {/* Application Notes snippet */}
                  {primaryApplication.notes && (
                    <div className="rounded-lg bg-background/60 p-2.5 text-xs text-muted-foreground border border-border/40">
                      <span className="font-semibold text-foreground block mb-0.5">Notes:</span>
                      <p className="line-clamp-3 italic">{primaryApplication.notes}</p>
                    </div>
                  )}
                </div>

                {/* Actions for Existing Application */}
                <div className="space-y-2">
                  <button
                    type="button"
                    onClick={() => handleOpenApplicationModal(primaryApplication)}
                    className="w-full inline-flex items-center justify-center gap-2 rounded-xl bg-accent px-4 py-2.5 text-xs font-semibold text-foreground hover:bg-accent/80 hover:text-primary transition-colors cursor-pointer"
                  >
                    <span>View Application & Transitions</span>
                    <ArrowRight className="h-3.5 w-3.5" />
                  </button>

                  <Link
                    to="/applications"
                    className="w-full inline-flex items-center justify-center gap-1.5 rounded-xl border border-border/60 bg-card px-4 py-2 text-xs font-medium text-muted-foreground hover:text-foreground transition-colors"
                  >
                    <span>Open Applications Board</span>
                    <ExternalLink className="h-3 w-3" />
                  </Link>
                </div>
              </div>
            )}
          </div>

          {/* Quick Actions Card */}
          <div className="rounded-xl border border-border/60 bg-card p-5 shadow-sm space-y-3">
            <h4 className="text-xs font-bold uppercase tracking-wider text-muted-foreground border-b border-border/40 pb-2">
              Quick Actions
            </h4>
            <div className="space-y-2 text-xs font-medium">
              <button
                type="button"
                onClick={handleEditClick}
                className="w-full flex items-center justify-between rounded-lg p-2 text-muted-foreground hover:bg-accent hover:text-foreground transition-colors cursor-pointer text-left"
              >
                <span className="flex items-center gap-2">
                  <Edit3 className="h-3.5 w-3.5 text-primary" /> Edit Vacancy Details
                </span>
                <ChevronRight className="h-3.5 w-3.5" />
              </button>

              {vacancy.url && (
                <a
                  href={vacancy.url}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="w-full flex items-center justify-between rounded-lg p-2 text-muted-foreground hover:bg-accent hover:text-foreground transition-colors text-left"
                >
                  <span className="flex items-center gap-2">
                    <ExternalLink className="h-3.5 w-3.5 text-primary" /> View Original Job Post
                  </span>
                  <ChevronRight className="h-3.5 w-3.5" />
                </a>
              )}

              <Link
                to="/vacancies"
                className="w-full flex items-center justify-between rounded-lg p-2 text-muted-foreground hover:bg-accent hover:text-foreground transition-colors text-left"
              >
                <span className="flex items-center gap-2">
                  <Briefcase className="h-3.5 w-3.5 text-primary" /> All Vacancies
                </span>
                <ChevronRight className="h-3.5 w-3.5" />
              </Link>
            </div>
          </div>
        </div>
      </div>

      {/* Application Detail Modal for auditing / status transitions */}
      <ApplicationDetailModal
        isOpen={isDetailModalOpen}
        onClose={() => setIsDetailModalOpen(false)}
        application={selectedApplication}
      />
    </div>
  );
};

export default VacancyDetailPage;
