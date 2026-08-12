import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import {
  useResumeRevision,
  useResumeRevisions,
  useCreateResumeDraft,
  useUpdateResumeDraft,
  useSpawnResumeVersion,
} from '@/api/resumes';
import { useMasterSkills } from '@/api/masterSkills';
import ErrorAlert from '@/components/ui/ErrorAlert';
import ResumeStatusBadge from '@/components/resumes/ResumeStatusBadge';
import PersonalInfoSection from '@/components/resumes/PersonalInfoSection';
import ProfessionalSummarySection from '@/components/resumes/ProfessionalSummarySection';
import SkillsSection from '@/components/resumes/SkillsSection';
import ResumeRevisionWorkflowCard from '@/components/resumes/ResumeRevisionWorkflowCard';
import ResumeVersionHistoryModal from '@/components/resumes/ResumeVersionHistoryModal';
import type {
  PersonalInfo,
  ResumeSkillInputDto,
  CreateResumeDraftDto,
  UpdateResumeDraftDto,
  ResumeTrack,
  CareerLevel,
} from '@/types';
import {
  ArrowLeft,
  Save,
  User,
  Award,
  Loader2,
  CheckCircle2,
  Briefcase,
  Sparkles,
  GraduationCap,
  FolderGit2,
  Languages,
  Layers,
  Lock,
  GitBranch,
  Plus,
  History,
} from 'lucide-react';

export type EditorSection =
  | 'all'
  | 'general'
  | 'experience'
  | 'education'
  | 'projects'
  | 'skills'
  | 'languages';

interface NavSectionItem {
  id: EditorSection;
  label: string;
  icon: React.ElementType;
  isImplemented: boolean;
}

const EDITOR_NAV_ITEMS: NavSectionItem[] = [
  { id: 'all', label: 'All Sections', icon: Layers, isImplemented: true },
  { id: 'general', label: 'General', icon: User, isImplemented: true },
  { id: 'experience', label: 'Experience', icon: Briefcase, isImplemented: false },
  { id: 'education', label: 'Education', icon: GraduationCap, isImplemented: false },
  { id: 'projects', label: 'Projects', icon: FolderGit2, isImplemented: false },
  { id: 'skills', label: 'Skills', icon: Award, isImplemented: true },
  { id: 'languages', label: 'Languages', icon: Languages, isImplemented: false },
];

export const ResumeEditorPage: React.FC = () => {
  const { resumeId } = useParams<{ resumeId: string }>();
  const navigate = useNavigate();
  const isNewMode = !resumeId || resumeId === 'new';

  // Section Navigation State
  const [activeSection, setActiveSection] = useState<EditorSection>('all');

  // History Modal State
  const [isHistoryModalOpen, setIsHistoryModalOpen] = useState(false);

  // API Hooks
  const {
    data: existingRevision,
    isLoading: isLoadingRevision,
    isError: isErrorRevision,
    error: loadError,
  } = useResumeRevision(isNewMode ? undefined : resumeId);
  const { data: allRevisions = [] } = useResumeRevisions();
  const createMutation = useCreateResumeDraft();
  const updateMutation = useUpdateResumeDraft();
  const spawnMutation = useSpawnResumeVersion();
  const { data: masterSkills = [] } = useMasterSkills();

  // Resume-level Metadata State
  const [name, setName] = useState('');
  const [track, setTrack] = useState<ResumeTrack>('Backend');
  const [careerLevel, setCareerLevel] = useState<CareerLevel>('Middle');
  const [targetRole, setTargetRole] = useState('');

  // Revision Content State
  const [personalInfo, setPersonalInfo] = useState<PersonalInfo>({
    fullName: '',
    email: '',
    phone: '',
    linkedIn: '',
    gitHub: '',
    location: '',
  });
  const [professionalSummary, setProfessionalSummary] = useState('');
  const [attachedSkills, setAttachedSkills] = useState<ResumeSkillInputDto[]>([]);

  // Status & Feedback State
  const [formError, setFormError] = useState<unknown | null>(null);
  const [showSaveSuccess, setShowSaveSuccess] = useState(false);

  // Populate state when editing existing revision
  useEffect(() => {
    if (!isNewMode && existingRevision) {
      setPersonalInfo({
        fullName: existingRevision.personalInfo?.fullName || '',
        email: existingRevision.personalInfo?.email || '',
        phone: existingRevision.personalInfo?.phone || '',
        linkedIn: existingRevision.personalInfo?.linkedIn || '',
        gitHub: existingRevision.personalInfo?.gitHub || '',
        location: existingRevision.personalInfo?.location || '',
      });
      setProfessionalSummary(existingRevision.professionalSummary || '');
      setAttachedSkills(
        existingRevision.skills?.map((s) => ({
          masterSkillId: s.masterSkillId,
          proficiencyLevel: s.proficiencyLevel,
        })) || []
      );
    }
  }, [isNewMode, existingRevision]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setFormError(null);
    setShowSaveSuccess(false);

    const formattedPersonalInfo: PersonalInfo = {
      fullName: personalInfo.fullName.trim(),
      email: personalInfo.email.trim(),
      phone: personalInfo.phone?.trim() || null,
      linkedIn: personalInfo.linkedIn?.trim() || null,
      gitHub: personalInfo.gitHub?.trim() || null,
      location: personalInfo.location?.trim() || null,
    };

    if (isNewMode) {
      const createDto: CreateResumeDraftDto = {
        name: name.trim() || `${personalInfo.fullName.trim() || 'Software Developer'}'s Resume`,
        track,
        careerLevel,
        targetRole: targetRole.trim() || 'Software Engineer',
        personalInfo: formattedPersonalInfo,
        professionalSummary: professionalSummary.trim(),
        skills: attachedSkills,
      };

      createMutation.mutate(createDto, {
        onSuccess: (createdRevision) => {
          navigate(`/resumes/${createdRevision.id}`, { replace: true });
        },
        onError: (err) => {
          setFormError(err);
        },
      });
    } else if (resumeId) {
      const updateDto: UpdateResumeDraftDto = {
        personalInfo: formattedPersonalInfo,
        professionalSummary: professionalSummary.trim(),
        skills: attachedSkills,
      };

      updateMutation.mutate(
        { id: resumeId, dto: updateDto },
        {
          onSuccess: () => {
            setShowSaveSuccess(true);
            setTimeout(() => setShowSaveSuccess(false), 4000);
          },
          onError: (err) => {
            setFormError(err);
          },
        }
      );
    }
  };

  const handleSpawnVersion = () => {
    if (!existingRevision || existingRevision.status !== 'Applied') return;
    setFormError(null);
    spawnMutation.mutate(existingRevision.id, {
      onSuccess: (newRev) => {
        navigate(`/resumes/${newRev.id}`);
      },
      onError: (err) => {
        setFormError(err);
      },
    });
  };

  const isSubmitting = createMutation.isPending || updateMutation.isPending;
  const isAppliedReadOnly = !isNewMode && existingRevision?.status === 'Applied';

  if (!isNewMode && isLoadingRevision) {
    return (
      <div className="h-64 rounded-xl border border-border/60 bg-card p-8 flex items-center justify-center animate-pulse">
        <div className="flex items-center gap-3 text-muted-foreground text-sm font-medium">
          <Loader2 className="h-5 w-5 animate-spin text-primary" />
          <span>Loading resume revision details...</span>
        </div>
      </div>
    );
  }

  if (!isNewMode && isErrorRevision) {
    return (
      <div className="space-y-4">
        <Link
          to="/resumes"
          className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
        >
          <ArrowLeft className="h-4 w-4" /> Back to Resumes
        </Link>
        <ErrorAlert error={loadError} />
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-5xl mx-auto pb-12">
      {/* Header Bar */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-border/40 pb-5">
        <div className="flex items-center gap-3">
          <Link
            to="/resumes"
            className="p-2 rounded-xl bg-card border border-border/60 text-muted-foreground hover:text-foreground hover:bg-accent transition-colors"
            title="Back to Resumes"
          >
            <ArrowLeft className="h-5 w-5" />
          </Link>
          <div>
            <div className="flex items-center gap-2">
              <h1 className="text-2xl font-bold tracking-tight text-foreground">
                {isNewMode
                  ? 'Create New Resume Profile'
                  : `Resume Editor (Revision ${existingRevision?.version || 1})`}
              </h1>
              {!isNewMode && existingRevision && (
                <ResumeStatusBadge status={existingRevision.status} size="sm" />
              )}
            </div>
            <p className="text-sm text-muted-foreground">
              {isNewMode
                ? 'Configure positioning metadata and initial draft content.'
                : 'Modify contact details, professional summary, and normalized skills for this revision.'}
            </p>
          </div>
        </div>

        <div className="flex items-center gap-3">
          <Link
            to="/resumes"
            className="rounded-xl border border-border bg-background px-4 py-2 text-sm font-semibold text-foreground hover:bg-accent transition-colors"
          >
            Cancel
          </Link>

          {!isNewMode && existingRevision && (
            <>
              <button
                type="button"
                onClick={() => setIsHistoryModalOpen(true)}
                className="inline-flex items-center gap-1.5 rounded-xl border border-border bg-card px-3.5 py-2 text-sm font-semibold text-foreground hover:bg-accent transition-colors cursor-pointer"
                title="View version history"
              >
                <History className="h-4 w-4 text-muted-foreground" />
                <span className="hidden sm:inline">View History</span>
              </button>

              {existingRevision.status === 'Applied' && (
                <button
                  type="button"
                  onClick={handleSpawnVersion}
                  disabled={spawnMutation.isPending}
                  className="inline-flex items-center gap-1.5 rounded-xl border border-primary/30 bg-primary/10 px-4 py-2 text-sm font-semibold text-primary hover:bg-primary/20 transition-all cursor-pointer disabled:opacity-50"
                  title="Create a new version draft from this revision"
                >
                  {spawnMutation.isPending ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : (
                    <GitBranch className="h-4 w-4" />
                  )}
                  <span>Create New Version</span>
                </button>
              )}
            </>
          )}

          <button
            type="submit"
            form="resume-editor-form"
            disabled={isSubmitting || isAppliedReadOnly}
            className="inline-flex items-center gap-2 rounded-xl bg-primary px-5 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isSubmitting ? (
              <Loader2 className="h-4 w-4 animate-spin" />
            ) : (
              <Save className="h-4 w-4" />
            )}
            <span>{isNewMode ? 'Create Resume Draft' : 'Save Changes'}</span>
          </button>
        </div>
      </div>

      {/* Success Notification */}
      {showSaveSuccess && (
        <div className="flex items-center gap-3 rounded-xl border border-emerald-500/30 bg-emerald-500/10 p-4 text-emerald-400 text-sm font-medium">
          <CheckCircle2 className="h-5 w-5 shrink-0" />
          <span>Resume draft updated successfully. Changes saved to PostgreSQL.</span>
        </div>
      )}

      {/* Error Notification */}
      <ErrorAlert error={formError} onDismiss={() => setFormError(null)} />

      {/* Applied Read-only Notice */}
      {isAppliedReadOnly && (
        <div className="rounded-xl border border-amber-500/30 bg-amber-500/10 p-4 text-amber-300 text-sm flex flex-col sm:flex-row sm:items-center justify-between gap-3">
          <div>
            <p className="font-semibold flex items-center gap-2">
              <Sparkles className="h-4 w-4" /> Read-Only Revision (ADR 005)
            </p>
            <p className="text-xs text-amber-300/80 mt-1">
              This resume revision has been linked to an active job application and marked as Applied.
              To make changes, branch a new editable draft.
            </p>
          </div>
          <button
            type="button"
            onClick={handleSpawnVersion}
            disabled={spawnMutation.isPending}
            className="inline-flex items-center gap-1.5 rounded-lg bg-amber-500/20 text-amber-200 border border-amber-500/30 px-3.5 py-1.5 text-xs font-semibold hover:bg-amber-500/30 transition-all cursor-pointer shrink-0"
          >
            {spawnMutation.isPending ? (
              <Loader2 className="h-3.5 w-3.5 animate-spin" />
            ) : (
              <Plus className="h-3.5 w-3.5" />
            )}
            <span>Create New Version (v{existingRevision.version + 1})</span>
          </button>
        </div>
      )}

      {/* RESUME REVISION WORKFLOW CARD & CURRENT REVISION HEADER */}
      {!isNewMode && existingRevision && (
        <ResumeRevisionWorkflowCard
          currentRevision={existingRevision}
          allRevisions={allRevisions}
          onSelectRevision={(id) => navigate(`/resumes/${id}`)}
          onSpawnVersion={handleSpawnVersion}
          onOpenHistoryModal={() => setIsHistoryModalOpen(true)}
          isSpawning={spawnMutation.isPending}
        />
      )}

      {/* EDITOR SECTION NAVIGATION */}
      <div className="flex items-center gap-2 overflow-x-auto border-b border-border/40 pb-3">
        {EDITOR_NAV_ITEMS.map((sec) => {
          const Icon = sec.icon;
          const isActive = activeSection === sec.id;
          return (
            <button
              key={sec.id}
              type="button"
              onClick={() => setActiveSection(sec.id)}
              className={`flex items-center gap-2 px-3.5 py-2 rounded-xl text-xs font-semibold transition-all whitespace-nowrap ${
                isActive
                  ? 'bg-primary text-primary-foreground shadow-sm'
                  : sec.isImplemented
                  ? 'bg-accent/40 text-muted-foreground hover:text-foreground hover:bg-accent cursor-pointer'
                  : 'bg-accent/10 text-muted-foreground/50 border border-dashed border-border/40 cursor-pointer hover:bg-accent/20 hover:text-muted-foreground'
              }`}
            >
              <Icon className="h-3.5 w-3.5 shrink-0" />
              <span>{sec.label}</span>
              {!sec.isImplemented && (
                <span className="rounded bg-muted/80 px-1.5 py-0.5 text-[10px] font-medium text-muted-foreground border border-border/40">
                  Soon
                </span>
              )}
            </button>
          );
        })}
      </div>

      <form id="resume-editor-form" onSubmit={handleSubmit} className="space-y-6">
        {/* RESUME INFORMATION (Resume Aggregate Metadata) */}
        {(activeSection === 'all' || activeSection === 'general') && (
          <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
            <div className="flex items-center gap-2 border-b border-border/40 pb-3">
              <div className="p-2 rounded-lg bg-primary/10 text-primary">
                <Briefcase className="h-4 w-4" />
              </div>
              <div>
                <h2 className="text-base font-semibold text-foreground">Resume Information</h2>
                <p className="text-xs text-muted-foreground">
                  Resume aggregate positioning metadata (Name, Target Role, Track, and Career Level).
                </p>
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
              <div>
                <label className="block text-xs font-medium text-muted-foreground mb-1">
                  Resume Name <span className="text-destructive">*</span>
                </label>
                <input
                  type="text"
                  required={isNewMode}
                  disabled={!isNewMode}
                  value={
                    isNewMode
                      ? name
                      : existingRevision?.personalInfo?.fullName
                      ? `${existingRevision.personalInfo.fullName}'s Profile`
                      : 'Resume Profile'
                  }
                  onChange={(e) => setName(e.target.value)}
                  placeholder="e.g. Senior .NET Backend Profile"
                  className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 disabled:bg-accent/30"
                />
              </div>

              <div>
                <label className="block text-xs font-medium text-muted-foreground mb-1">
                  Target Role <span className="text-destructive">*</span>
                </label>
                <input
                  type="text"
                  required={isNewMode}
                  disabled={!isNewMode}
                  value={isNewMode ? targetRole : '.NET Software Engineer'}
                  onChange={(e) => setTargetRole(e.target.value)}
                  placeholder="e.g. Senior C# Backend Engineer"
                  className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 disabled:bg-accent/30"
                />
              </div>

              <div>
                <label className="block text-xs font-medium text-muted-foreground mb-1">
                  Track
                </label>
                <select
                  disabled={!isNewMode}
                  value={track}
                  onChange={(e) => setTrack(e.target.value as ResumeTrack)}
                  className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm font-medium text-foreground focus:outline-none focus:ring-2 focus:ring-primary cursor-pointer disabled:opacity-60 disabled:bg-accent/30"
                >
                  <option value="Backend">Backend</option>
                  <option value="Frontend">Frontend</option>
                  <option value="FullStack">FullStack</option>
                </select>
              </div>

              <div>
                <label className="block text-xs font-medium text-muted-foreground mb-1">
                  Career Level
                </label>
                <select
                  disabled={!isNewMode}
                  value={careerLevel}
                  onChange={(e) => setCareerLevel(e.target.value as CareerLevel)}
                  className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm font-medium text-foreground focus:outline-none focus:ring-2 focus:ring-primary cursor-pointer disabled:opacity-60 disabled:bg-accent/30"
                >
                  <option value="Intern">Intern</option>
                  <option value="Junior">Junior</option>
                  <option value="Middle">Middle</option>
                  <option value="Senior">Senior</option>
                  <option value="Lead">Lead</option>
                </select>
              </div>
            </div>
          </div>
        )}

        {/* GENERAL SECTION (Personal Information & Professional Summary) */}
        {(activeSection === 'all' || activeSection === 'general') && (
          <>
            <PersonalInfoSection
              personalInfo={personalInfo}
              onChange={setPersonalInfo}
              isReadOnly={isAppliedReadOnly}
            />

            <ProfessionalSummarySection
              summary={professionalSummary}
              onChange={setProfessionalSummary}
              isReadOnly={isAppliedReadOnly}
            />
          </>
        )}

        {/* SKILLS SECTION */}
        {(activeSection === 'all' || activeSection === 'skills') && (
          <SkillsSection
            attachedSkills={attachedSkills}
            onChangeAttachedSkills={setAttachedSkills}
            masterSkills={masterSkills}
            isReadOnly={isAppliedReadOnly}
          />
        )}

        {/* NON-INTERACTIVE / FUTURE SECTIONS PLACEHOLDER */}
        {!['all', 'general', 'skills'].includes(activeSection) && (
          <div className="rounded-xl border border-dashed border-border/80 bg-card p-12 text-center shadow-sm">
            <div className="mx-auto w-12 h-12 rounded-full bg-accent/50 flex items-center justify-center text-muted-foreground mb-3">
              <Lock className="h-6 w-6" />
            </div>
            <h3 className="text-base font-semibold text-foreground capitalize">
              {activeSection} Section
            </h3>
            <p className="mt-1 text-sm text-muted-foreground max-w-md mx-auto">
              This section is scheduled for future release (Phase 2). Work experiences, education history, portfolio projects, and spoken languages will be enabled in upcoming versions.
            </p>
            <div className="mt-4">
              <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-primary/10 text-primary text-xs font-semibold border border-primary/20">
                <Sparkles className="h-3.5 w-3.5" /> Future Section — Non-Interactive
              </span>
            </div>
          </div>
        )}
      </form>

      {/* REVISION VERSION HISTORY MODAL */}
      {!isNewMode && existingRevision && (
        <ResumeVersionHistoryModal
          isOpen={isHistoryModalOpen}
          onClose={() => setIsHistoryModalOpen(false)}
          revisions={allRevisions}
          selectedRevision={existingRevision}
          onSelectRevision={(rev) => {
            setIsHistoryModalOpen(false);
            navigate(`/resumes/${rev.id}`);
          }}
        />
      )}
    </div>
  );
};

export default ResumeEditorPage;
