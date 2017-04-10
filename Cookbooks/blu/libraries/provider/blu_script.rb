require "chef/provider/script"

class Chef
  class Provider
    class BluScript < Chef::Provider::WindowsScript
      provides :blu_script, os: 'windows'

      def initialize(new_resource, run_context)
        super(new_resource, run_context, '.ps1')
        if code.nil?
          @code = ''
        end
      end

      def command
        cmd = "\"#{interpreter}\" exec --script-file \"#{script_file.path}\""
        unless @new_resource.runspace.nil? || @new_resource.runspace.empty?
          cmd << " --runspace \"#{@new_resource.runspace}\""
        end
        cmd
      end

      def action_create_runspace
        cmd = "\"#{interpreter}\" runspace \"#{@new_resource.runspace}\""
        unless @new_resource.credentials.nil? || @new_resource.credentials.empty?
          cmd << " --credentials \"#{@new_resource.credentials}\""
        end

        converge_by("create runspace #{cmd}") do
          begin
            shell_out!(cmd, opts)
          rescue Mixlib::ShellOut::ShellCommandFailed
            if sensitive?
              raise Mixlib::ShellOut::ShellCommandFailed,
                    "Command execution failed. STDOUT/STDERR suppressed for sensitive resource"
            else
              raise
            end
          end
          Chef::Log.info("#{new_resource} ran successfully")
        end
      end

      def action_dispose_runspace
        cmd = "\"#{interpreter}\" runspace --dispose \"#{@new_resource.runspace}\""

        converge_by("dispose runspace #{cmd}") do
          begin
            shell_out!(cmd, opts)
          rescue Mixlib::ShellOut::ShellCommandFailed
            if sensitive?
              raise Mixlib::ShellOut::ShellCommandFailed,
                    "Command execution failed. STDOUT/STDERR suppressed for sensitive resource"
            else
              raise
            end
          end
          Chef::Log.info("#{new_resource} ran successfully")
        end
      end
    end
  end
end
