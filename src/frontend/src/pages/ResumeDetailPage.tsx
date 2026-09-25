import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useResumeRevision, useSpawnResumeVersion } from '@/api/resumes';
import { useEducations } from '@/api/educations';
import { useProjects } from '@/api/projects';
import ResumeStatusBadge from '@/components/resumes/ResumeStatusBadge';
import {
  ArrowLeft,
  User,
  Mail,
  Phone,
  MapPin,
  Linkedin,
  Github,
  FileText,
  Award,
  GitBranch,
  Edit3,
  Lock,
  Calendar,
  CheckCircle,
  GraduationCap,
  FolderGit2,
  Globe,
  ExternalLink,
  Code2,
  AlertCircle,
  FileUser,
} from 'lucide-react';

export const ResumeDetailPage: React.FC = () => {
  const { revisionId } = useParams<{ revisionId: string }>();
  const navigate = useNavigate();

  const {
    data: revision,
    isLoading,
    isError,
    error,
    refetch,
  } = useResumeRevision(revisionId);

  const { data: educations = [], isLoading: isLoadingEducations } = useEducations(revision?.id);
  const { data: projects = [], isLoading: isLoadingProjects } = useProjects(revision?.id);

  const spawnMutation = useSpawnResumeVersion();

  const handleSpawn = () => {
    if (!revision || revision.status !== 'Applied') return;
    spawnMutation.mutate(revision.id, {
      onSuccess: (newRevision) => {
        navigate(`/resumes/${newRevision.id}/details`);
      },
    });
  };

  const handleEdit = () => {
    if (!revision) return;
    navigate(`/resumes/${revision.id}`);
  };

  // Loading State
  if (isLoading) {
    return (
      <div className="space-y-6 max-w-6xl mx-auto py-4">
        <div className="flex items-center gap-3">
          <div className="h-8 w-36 rounded-lg bg-accent/60 animate-pulse" />
        </div>
        <div className="h-28 rounded-xl border border-border/60 bg-card p-6 animate-pulse space-y-4">
          <div className="h-6 w-1/3 bg-accent/60 rounded" />
          <div className="h-4 w-1/4 bg-accent/40 rounded" />
        </div>
        <div className="space-y-4">
          <div className="h-40 rounded-xl border border-border/60 bg-card animate-pulse" />
          <div className="h-32 rounded-xl border border-border/60 bg-card animate-pulse" />
          <div className="h-48 rounded-xl border border-border/60 bg-card animate-pulse" />
        </div>
      </div>
    );
  }

  // Error State
  if (isError) {
    return (
      <div className="space-y-6 max-w-6xl mx-auto py-4">
        <button
          type="button"
          onClick={() => navigate('/resumes')}
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors cursor-pointer"
        >
          <ArrowLeft className="h-4 w-4" /> Back to Resumes
        </button>
        <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
          <AlertCircle className="mx-auto h-10 w-10 text-destructive mb-3" />
          <h3 className="text-base font-semibold text-destructive">Failed to load resume details</h3>
          <p className="text-xs text-muted-foreground mt-1 mb-4">
            {error instanceof Error ? error.message : 'An error occurred while fetching the resume revision.'}
          </p>
          <button
            type="button"
            onClick={() => refetch()}
            className="rounded-lg bg-destructive px-4 py-2 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90 transition-colors"
          >
            Retry Loading
          </button>
        </div>
      </div>
    );
  }

  // Not Found State
  if (!revision) {
    return (
      <div className="space-y-6 max-w-6xl mx-auto py-4">
        <button
          type="button"
          onClick={() => navigate('/resumes')}
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors cursor-pointer"
        >
          <ArrowLeft className="h-4 w-4" /> Back to Resumes
        </button>
        <div className="rounded-xl border border-dashed border-border p-12 text-center shadow-sm">
          <FileUser className="mx-auto h-12 w-12 text-muted-foreground/40 mb-3" />
          <h3 className="text-base font-semibold text-foreground">Resume Revision Not Found</h3>
          <p className="mt-1 text-sm text-muted-foreground">
            The requested resume revision does not exist or has been removed.
          </p>
          <button
            type="button"
            onClick={() => navigate('/resumes')}
            className="mt-4 inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all"
          >
            <ArrowLeft className="h-3.5 w-3.5" /> Back to Resumes
          </button>
        </div>
      </div>
    );
  }

  const isDraft = revision.status === 'Draft';
  const { personalInfo } = revision;

  return (
    <div className="space-y-5 max-w-6xl mx-auto py-4">
      {/* Back Navigation */}
      <div className="flex items-center justify-between">
        <button
          type="button"
          onClick={() => navigate('/resumes')}
          className="inline-flex items-center gap-2 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors cursor-pointer"
        >
          <ArrowLeft className="h-4 w-4" /> Back to Resumes
        </button>
        <span className="text-xs text-muted-foreground">
          Revision ID: {revision.id}
        </span>
      </div>

      {/* Immutability & Status Alert Banner per ADR 005 */}
      {!isDraft ? (
        <div className="rounded-lg border border-emerald-500/30 bg-emerald-500/10 p-3.5 text-xs text-emerald-300 flex items-start gap-3">
          <Lock className="h-5 w-5 text-emerald-400 shrink-0 mt-0.5" />
          <div>
            <h4 className="font-semibold text-emerald-400 text-sm">
              Immutable Applied Revision (ADR 005)
            </h4>
            <p className="mt-0.5 opacity-90">
              This revision has been submitted with an application and is strictly read-only to guarantee historical application accuracy. To make edits, click <strong>Spawn New Version</strong> below to create a new draft copy (Copy-on-Write).
            </p>
          </div>
        </div>
      ) : (
        <div className="rounded-lg border border-amber-500/30 bg-amber-500/10 p-3.5 text-xs text-amber-300 flex items-start gap-3">
          <CheckCircle className="h-5 w-5 text-amber-400 shrink-0 mt-0.5" />
          <div>
            <h4 className="font-semibold text-amber-400 text-sm">Editable Draft Revision</h4>
            <p className="mt-0.5 opacity-90">
              This revision is in Draft status. You can modify its contact info, summary, or skills directly.
            </p>
          </div>
        </div>
      )}

      {/* Section 1: Overview Header */}
      <div className="flex flex-wrap items-center justify-between gap-3 rounded-lg border border-border/40 bg-accent/20 p-4">
        <div className="space-y-1">
          <div className="flex items-center gap-2 flex-wrap">
            <span className="text-xl font-bold text-foreground">
              {revision.resumeName || revision.personalInfo?.fullName || 'Untitled Profile'}
            </span>
            <span className="inline-flex items-center px-2.5 py-0.5 rounded-md bg-primary/10 text-primary text-xs font-bold border border-primary/20">
              v{revision.version}
            </span>
          </div>
          {(revision.targetRole || revision.track || revision.careerLevel) && (
            <div className="text-xs font-semibold text-primary flex items-center gap-1.5 flex-wrap">
              {revision.targetRole || `${revision.track} Developer`}
              {revision.track && revision.careerLevel && (
                <span className="text-muted-foreground text-[11px] font-normal">
                  • {revision.track} ({revision.careerLevel})
                </span>
              )}
            </div>
          )}
          <p className="text-xs text-muted-foreground">{personalInfo?.fullName} {personalInfo?.email && `• ${personalInfo.email}`}</p>
        </div>

        <div className="flex items-center gap-2">
          <ResumeStatusBadge status={revision.status} size="md" />
        </div>
      </div>

      {/* Section 2: Contact Information */}
      <div className="space-y-2 rounded-lg border border-border/40 bg-card p-4">
        <div className="flex items-center gap-2 text-xs font-semibold text-foreground border-b border-border/40 pb-2">
          <User className="h-4 w-4 text-primary" />
          <span>Contact Information</span>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-2.5 text-xs pt-1">
          {personalInfo?.email && (
            <div className="flex items-center gap-2 text-muted-foreground">
              <Mail className="h-3.5 w-3.5 text-primary shrink-0" />
              <span className="font-medium text-foreground">Email:</span> {personalInfo.email}
            </div>
          )}
          {personalInfo?.phone && (
            <div className="flex items-center gap-2 text-muted-foreground">
              <Phone className="h-3.5 w-3.5 text-primary shrink-0" />
              <span className="font-medium text-foreground">Phone:</span> {personalInfo.phone}
            </div>
          )}
          {personalInfo?.location && (
            <div className="flex items-center gap-2 text-muted-foreground">
              <MapPin className="h-3.5 w-3.5 text-primary shrink-0" />
              <span className="font-medium text-foreground">Location:</span> {personalInfo.location}
            </div>
          )}
          {personalInfo?.linkedIn && (
            <div className="flex items-center gap-2 text-muted-foreground truncate">
              <Linkedin className="h-3.5 w-3.5 text-blue-400 shrink-0" />
              <span className="font-medium text-foreground">LinkedIn:</span>
              <a
                href={personalInfo.linkedIn}
                target="_blank"
                rel="noreferrer"
                className="text-primary hover:underline truncate"
              >
                {personalInfo.linkedIn}
              </a>
            </div>
          )}
          {personalInfo?.gitHub && (
            <div className="flex items-center gap-2 text-muted-foreground truncate">
              <Github className="h-3.5 w-3.5 text-foreground shrink-0" />
              <span className="font-medium text-foreground">GitHub:</span>
              <a
                href={personalInfo.gitHub}
                target="_blank"
                rel="noreferrer"
                className="text-primary hover:underline truncate"
              >
                {personalInfo.gitHub}
              </a>
            </div>
          )}
        </div>
      </div>

      {/* Section 3: Professional Summary */}
      <div className="space-y-2 rounded-lg border border-border/40 bg-card p-4">
        <div className="flex items-center gap-2 text-xs font-semibold text-foreground border-b border-border/40 pb-2">
          <FileText className="h-4 w-4 text-primary" />
          <span>Professional Summary</span>
        </div>
        <p className="text-xs text-muted-foreground leading-relaxed whitespace-pre-line pt-1">
          {revision.professionalSummary}
        </p>
      </div>

      {/* Section 4: Attached Master Skills */}
      <div className="space-y-2 rounded-lg border border-border/40 bg-card p-4">
        <div className="flex items-center justify-between border-b border-border/40 pb-2">
          <div className="flex items-center gap-2 text-xs font-semibold text-foreground">
            <Award className="h-4 w-4 text-primary" />
            <span>Normalized Technical Skills (ADR 006)</span>
          </div>
          <span className="text-xs text-muted-foreground">
            {revision.skills?.length || 0} skills
          </span>
        </div>

        {revision.skills && revision.skills.length > 0 ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 pt-1">
            {revision.skills.map((skill) => (
              <div
                key={skill.masterSkillId}
                className="flex items-center justify-between rounded-lg bg-accent/30 border border-border/40 px-3 py-2 text-xs"
              >
                <div className="flex items-center gap-2">
                  <span className="font-semibold text-foreground">{skill.skillName}</span>
                  <span className="text-[10px] text-muted-foreground bg-accent px-1.5 py-0.5 rounded">
                    {skill.category}
                  </span>
                </div>

                <div className="flex items-center gap-1">
                  <span className="text-[11px] font-bold text-primary">
                    Level {skill.proficiencyLevel}/5
                  </span>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p className="text-xs text-muted-foreground italic text-center py-2">
            No skills attached to this revision.
          </p>
        )}
      </div>

      {/* Section 5: Academic Credentials & Education */}
      <div className="space-y-2 rounded-lg border border-border/40 bg-card p-4">
        <div className="flex items-center justify-between border-b border-border/40 pb-2">
          <div className="flex items-center gap-2 text-xs font-semibold text-foreground">
            <GraduationCap className="h-4 w-4 text-primary" />
            <span>Education History (ADR 010)</span>
          </div>
          <span className="text-xs text-muted-foreground">
            {educations.length} credential(s)
          </span>
        </div>

        {isLoadingEducations ? (
          <div className="py-3 text-center text-xs text-muted-foreground animate-pulse">
            Loading education history...
          </div>
        ) : educations.length > 0 ? (
          <div className="space-y-2 pt-1">
            {educations.map((edu) => (
              <div
                key={edu.id}
                className="flex flex-col gap-1 rounded-lg bg-accent/30 border border-border/40 p-3 text-xs"
              >
                <div className="flex items-start justify-between gap-2">
                  <span className="font-semibold text-foreground">{edu.institutionName}</span>
                  {(edu.startYear || edu.endYear) && (
                    <span className="text-[11px] font-medium text-muted-foreground shrink-0 flex items-center gap-1">
                      <Calendar className="h-3 w-3 text-primary" />
                      {edu.startYear && edu.endYear
                        ? `${edu.startYear} — ${edu.endYear}`
                        : edu.startYear
                        ? `Since ${edu.startYear}`
                        : `Completed ${edu.endYear}`}
                    </span>
                  )}
                </div>
                {edu.description && (
                  <p className="text-muted-foreground text-xs whitespace-pre-line">
                    {edu.description}
                  </p>
                )}
              </div>
            ))}
          </div>
        ) : (
          <p className="text-xs text-muted-foreground italic text-center py-2">
            No education history attached to this revision.
          </p>
        )}
      </div>

      {/* Section 6: Projects & Portfolio */}
      <div className="space-y-2 rounded-lg border border-border/40 bg-card p-4">
        <div className="flex items-center justify-between border-b border-border/40 pb-2">
          <div className="flex items-center gap-2 text-xs font-semibold text-foreground">
            <FolderGit2 className="h-4 w-4 text-primary" />
            <span>Projects &amp; Portfolio (ADR 010)</span>
          </div>
          <span className="text-xs text-muted-foreground">
            {projects.length} project(s)
          </span>
        </div>

        {isLoadingProjects ? (
          <div className="py-3 text-center text-xs text-muted-foreground animate-pulse">
            Loading projects...
          </div>
        ) : projects.length > 0 ? (
          <div className="space-y-2 pt-1">
            {projects.map((proj) => {
              const techStackList = proj.techStack
                ? proj.techStack
                    .split(',')
                    .map((s) => s.trim())
                    .filter(Boolean)
                : [];

              return (
                <div
                  key={proj.id}
                  className="flex flex-col gap-1.5 rounded-lg bg-accent/30 border border-border/40 p-3 text-xs"
                >
                  <div className="flex items-start justify-between gap-2">
                    <div className="flex items-center gap-2 flex-wrap">
                      <span className="font-semibold text-foreground">{proj.projectName}</span>
                      {proj.role && (
                        <span className="inline-flex items-center px-2 py-0.5 rounded-md text-[10px] font-medium bg-secondary text-secondary-foreground border border-border/50">
                          {proj.role}
                        </span>
                      )}
                    </div>
                  </div>

                  {proj.description && (
                    <p className="text-muted-foreground text-xs whitespace-pre-line leading-relaxed">
                      {proj.description}
                    </p>
                  )}

                  {techStackList.length > 0 && (
                    <div className="flex flex-wrap items-center gap-1.5 pt-0.5">
                      {techStackList.map((tech, idx) => (
                        <span
                          key={idx}
                          className="inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[10px] font-medium bg-accent/60 text-accent-foreground border border-border/40"
                        >
                          <Code2 className="h-2.5 w-2.5 text-primary" />
                          {tech}
                        </span>
                      ))}
                    </div>
                  )}

                  {(proj.repositoryUrl || proj.liveDemoUrl) && (
                    <div className="flex flex-wrap items-center gap-3 pt-1 text-[11px]">
                      {proj.repositoryUrl && (
                        <a
                          href={proj.repositoryUrl}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="inline-flex items-center gap-1 text-muted-foreground hover:text-primary transition-colors font-medium"
                        >
                          <Github className="h-3 w-3" />
                          <span>Repository</span>
                          <ExternalLink className="h-2.5 w-2.5 opacity-60" />
                        </a>
                      )}
                      {proj.liveDemoUrl && (
                        <a
                          href={proj.liveDemoUrl}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="inline-flex items-center gap-1 text-muted-foreground hover:text-primary transition-colors font-medium"
                        >
                          <Globe className="h-3 w-3" />
                          <span>Live Demo</span>
                          <ExternalLink className="h-2.5 w-2.5 opacity-60" />
                        </a>
                      )}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        ) : (
          <p className="text-xs text-muted-foreground italic text-center py-2">
            No portfolio projects attached to this revision.
          </p>
        )}
      </div>

      {/* Section 7: Revision Metadata & Lineage */}
      <div className="rounded-lg border border-border/40 bg-accent/10 p-3 text-[11px] text-muted-foreground space-y-1">
        <div className="flex flex-wrap items-center justify-between gap-2">
          <div className="flex items-center gap-1.5">
            <Calendar className="h-3.5 w-3.5 text-primary shrink-0" />
            <span>Created: {new Date(revision.createdAt).toLocaleString()}</span>
          </div>
          <div className="flex items-center gap-1.5">
            <span>Updated: {new Date(revision.updatedAt).toLocaleString()}</span>
          </div>
        </div>
        {revision.parentRevisionId && (
          <div className="flex items-center gap-1.5 pt-1 border-t border-border/20 text-primary">
            <GitBranch className="h-3.5 w-3.5 shrink-0" />
            <span>Spawned from parent revision ID: {revision.parentRevisionId}</span>
          </div>
        )}
      </div>

      {/* Actions Footer */}
      <div className="flex items-center justify-between pt-3 border-t border-border/40">
        <button
          type="button"
          onClick={() => navigate('/resumes')}
          className="inline-flex items-center gap-2 rounded-lg border border-border bg-background px-4 py-2 text-sm font-semibold text-foreground hover:bg-accent transition-colors"
        >
          <ArrowLeft className="h-4 w-4" />
          Back to Resumes
        </button>

        <div className="flex items-center gap-2">
          {isDraft && (
            <button
              type="button"
              onClick={handleEdit}
              className="inline-flex items-center gap-1.5 rounded-lg bg-amber-500/10 border border-amber-500/30 px-4 py-2 text-sm font-semibold text-amber-400 hover:bg-amber-500/20 transition-colors cursor-pointer"
            >
              <Edit3 className="h-4 w-4" />
              Edit Draft
            </button>
          )}

          {!isDraft && (
            <button
              type="button"
              disabled={spawnMutation.isPending}
              onClick={handleSpawn}
              className="inline-flex items-center gap-1.5 rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer disabled:opacity-50"
            >
              <GitBranch className="h-4 w-4" />
              {spawnMutation.isPending ? 'Spawning...' : 'Spawn New Version'}
            </button>
          )}
        </div>
      </div>
    </div>
  );
};

export default ResumeDetailPage;
