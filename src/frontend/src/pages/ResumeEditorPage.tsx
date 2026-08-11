import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import {
  useResumeRevision,
  useCreateResumeDraft,
  useUpdateResumeDraft,
} from '@/api/resumes';
import { useMasterSkills } from '@/api/masterSkills';
import CustomSelect from '@/components/ui/CustomSelect';
import ErrorAlert from '@/components/ui/ErrorAlert';
import ResumeStatusBadge from '@/components/resumes/ResumeStatusBadge';
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
  FileText,
  Award,
  Plus,
  Trash2,
  Loader2,
  CheckCircle2,
  Briefcase,
  Sparkles,
} from 'lucide-react';

export const ResumeEditorPage: React.FC = () => {
  const { resumeId } = useParams<{ resumeId: string }>();
  const navigate = useNavigate();
  const isNewMode = !resumeId || resumeId === 'new';

  // API Hooks
  const {
    data: existingRevision,
    isLoading: isLoadingRevision,
    isError: isErrorRevision,
    error: loadError,
  } = useResumeRevision(isNewMode ? undefined : resumeId);
  const createMutation = useCreateResumeDraft();
  const updateMutation = useUpdateResumeDraft();
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

  // Skill Selector State
  const [selectedSkillId, setSelectedSkillId] = useState('');
  const [selectedProficiency, setSelectedProficiency] = useState(3);

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

  const handleAddSkill = () => {
    if (!selectedSkillId) return;
    if (attachedSkills.some((s) => s.masterSkillId === selectedSkillId)) {
      return;
    }
    setAttachedSkills([
      ...attachedSkills,
      { masterSkillId: selectedSkillId, proficiencyLevel: selectedProficiency },
    ]);
    setSelectedSkillId('');
    setSelectedProficiency(3);
  };

  const handleRemoveSkill = (skillId: string) => {
    setAttachedSkills(attachedSkills.filter((s) => s.masterSkillId !== skillId));
  };

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

  const isSubmitting = createMutation.isPending || updateMutation.isPending;
  const isAppliedReadOnly = !isNewMode && existingRevision?.status === 'Applied';

  const availableSkillOptions = masterSkills
    .filter((ms) => !attachedSkills.some((as) => as.masterSkillId === ms.id))
    .map((ms) => ({
      value: ms.id,
      label: `${ms.name} (${ms.category})`,
    }));

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
                  : `Resume Editor (v${existingRevision?.version || 1})`}
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
        <div className="rounded-xl border border-amber-500/30 bg-amber-500/10 p-4 text-amber-300 text-sm">
          <p className="font-semibold flex items-center gap-2">
            <Sparkles className="h-4 w-4" /> Read-Only Revision (ADR 005)
          </p>
          <p className="text-xs text-amber-300/80 mt-1">
            This resume revision has been linked to an active job application and marked as Applied.
            To make changes, return to the Resumes list and click &quot;Spawn&quot; to branch a new editable draft.
          </p>
        </div>
      )}

      <form id="resume-editor-form" onSubmit={handleSubmit} className="space-y-6">
        {/* SECTION 1: RESUME-LEVEL METADATA */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <div className="flex items-center gap-2 border-b border-border/40 pb-3">
            <div className="p-2 rounded-lg bg-primary/10 text-primary">
              <Briefcase className="h-4 w-4" />
            </div>
            <div>
              <h2 className="text-base font-semibold text-foreground">1. Resume Profile Metadata</h2>
              <p className="text-xs text-muted-foreground">
                High-level career positioning identity (Track, Career Level, and Target Role).
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
                value={isNewMode ? name : existingRevision?.personalInfo?.fullName ? `${existingRevision.personalInfo.fullName}'s Profile` : 'Resume Profile'}
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

        {/* SECTION 2: PERSONAL INFORMATION */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <div className="flex items-center gap-2 border-b border-border/40 pb-3">
            <div className="p-2 rounded-lg bg-primary/10 text-primary">
              <User className="h-4 w-4" />
            </div>
            <div>
              <h2 className="text-base font-semibold text-foreground">2. Personal Information</h2>
              <p className="text-xs text-muted-foreground">
                Developer contact details snapshot attached to this revision.
              </p>
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                Full Name <span className="text-destructive">*</span>
              </label>
              <input
                type="text"
                required
                disabled={isAppliedReadOnly}
                value={personalInfo.fullName}
                onChange={(e) => setPersonalInfo({ ...personalInfo, fullName: e.target.value })}
                placeholder="e.g. Alex Johnson"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                Email Address <span className="text-destructive">*</span>
              </label>
              <input
                type="email"
                required
                disabled={isAppliedReadOnly}
                value={personalInfo.email}
                onChange={(e) => setPersonalInfo({ ...personalInfo, email: e.target.value })}
                placeholder="e.g. alex.johnson@example.com"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                Phone Number
              </label>
              <input
                type="tel"
                disabled={isAppliedReadOnly}
                value={personalInfo.phone || ''}
                onChange={(e) => setPersonalInfo({ ...personalInfo, phone: e.target.value })}
                placeholder="e.g. +1 (555) 234-5678"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                Location
              </label>
              <input
                type="text"
                disabled={isAppliedReadOnly}
                value={personalInfo.location || ''}
                onChange={(e) => setPersonalInfo({ ...personalInfo, location: e.target.value })}
                placeholder="e.g. San Francisco, CA"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                LinkedIn Profile URL
              </label>
              <input
                type="url"
                disabled={isAppliedReadOnly}
                value={personalInfo.linkedIn || ''}
                onChange={(e) => setPersonalInfo({ ...personalInfo, linkedIn: e.target.value })}
                placeholder="https://linkedin.com/in/username"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60"
              />
            </div>

            <div>
              <label className="block text-xs font-medium text-muted-foreground mb-1">
                GitHub Profile URL
              </label>
              <input
                type="url"
                disabled={isAppliedReadOnly}
                value={personalInfo.gitHub || ''}
                onChange={(e) => setPersonalInfo({ ...personalInfo, gitHub: e.target.value })}
                placeholder="https://github.com/username"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60"
              />
            </div>
          </div>
        </div>

        {/* SECTION 3: PROFESSIONAL SUMMARY */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <div className="flex items-center gap-2 border-b border-border/40 pb-3">
            <div className="p-2 rounded-lg bg-primary/10 text-primary">
              <FileText className="h-4 w-4" />
            </div>
            <div>
              <h2 className="text-base font-semibold text-foreground">3. Professional Summary</h2>
              <p className="text-xs text-muted-foreground">
                Executive summary detailing core technical focus and career accomplishments.
              </p>
            </div>
          </div>

          <textarea
            required
            rows={5}
            disabled={isAppliedReadOnly}
            value={professionalSummary}
            onChange={(e) => setProfessionalSummary(e.target.value)}
            placeholder="Write a compelling overview of your software engineering expertise, core technical strengths, and career highlights..."
            className="w-full rounded-lg border border-border bg-background p-3 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60"
          />
        </div>

        {/* SECTION 4: SKILLS NORMALIZATION & CATALOG */}
        <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
          <div className="flex items-center justify-between border-b border-border/40 pb-3">
            <div className="flex items-center gap-2">
              <div className="p-2 rounded-lg bg-primary/10 text-primary">
                <Award className="h-4 w-4" />
              </div>
              <div>
                <h2 className="text-base font-semibold text-foreground">
                  4. Skills Catalog Normalization (ADR 006)
                </h2>
                <p className="text-xs text-muted-foreground">
                  Master skills attached to this revision snapshot.
                </p>
              </div>
            </div>
            <span className="text-xs text-muted-foreground font-medium">
              {attachedSkills.length} skill(s) attached
            </span>
          </div>

          {/* Add Skill Controls */}
          {!isAppliedReadOnly && (
            <div className="flex flex-col sm:flex-row items-stretch sm:items-center gap-3 bg-accent/20 p-4 rounded-xl border border-border/40">
              <div className="flex-1">
                <CustomSelect
                  value={selectedSkillId}
                  onChange={(val) => setSelectedSkillId(val)}
                  options={availableSkillOptions}
                  placeholder="Select skill from MasterSkill catalog..."
                />
              </div>

              <div className="flex items-center gap-3">
                <div className="flex items-center gap-2">
                  <span className="text-xs text-muted-foreground font-medium whitespace-nowrap">
                    Proficiency:
                  </span>
                  <select
                    value={selectedProficiency}
                    onChange={(e) => setSelectedProficiency(Number(e.target.value))}
                    className="rounded-lg border border-border bg-background px-3 py-2 text-xs font-semibold text-foreground focus:outline-none focus:ring-2 focus:ring-primary cursor-pointer"
                  >
                    <option value={1}>1 - Beginner</option>
                    <option value={2}>2 - Elementary</option>
                    <option value={3}>3 - Intermediate</option>
                    <option value={4}>4 - Advanced</option>
                    <option value={5}>5 - Expert</option>
                  </select>
                </div>

                <button
                  type="button"
                  onClick={handleAddSkill}
                  disabled={!selectedSkillId}
                  className="inline-flex items-center gap-1.5 rounded-lg bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <Plus className="h-3.5 w-3.5" /> Attach Skill
                </button>
              </div>
            </div>
          )}

          {/* Attached Skills Display */}
          {attachedSkills.length > 0 ? (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 pt-2">
              {attachedSkills.map((s) => {
                const masterSkill = masterSkills.find((ms) => ms.id === s.masterSkillId);
                const skillName = masterSkill ? masterSkill.name : s.masterSkillId;
                const category = masterSkill?.category || 'Other';

                return (
                  <div
                    key={s.masterSkillId}
                    className="flex items-center justify-between gap-2 rounded-xl bg-background border border-border/80 p-3 shadow-xs"
                  >
                    <div>
                      <div className="font-semibold text-sm text-foreground">{skillName}</div>
                      <div className="text-[11px] text-muted-foreground">{category}</div>
                    </div>

                    <div className="flex items-center gap-2">
                      <span className="rounded-md bg-primary/10 text-primary border border-primary/20 px-2 py-0.5 text-xs font-bold">
                        Lvl {s.proficiencyLevel}
                      </span>
                      {!isAppliedReadOnly && (
                        <button
                          type="button"
                          onClick={() => handleRemoveSkill(s.masterSkillId)}
                          className="text-muted-foreground hover:text-destructive transition-colors p-1"
                          title="Remove skill"
                        >
                          <Trash2 className="h-4 w-4" />
                        </button>
                      )}
                    </div>
                  </div>
                );
              })}
            </div>
          ) : (
            <div className="rounded-xl border border-dashed border-border/80 p-6 text-center text-xs text-muted-foreground italic">
              No skills attached yet. Select a skill from the MasterSkill catalog above.
            </div>
          )}
        </div>
      </form>
    </div>
  );
};

export default ResumeEditorPage;
