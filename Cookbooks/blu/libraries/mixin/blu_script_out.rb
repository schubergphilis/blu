require "chef/mixin/shell_out"
require "chef/mixin/windows_architecture_helper"

class Chef
  module Mixin
    module BluScriptOut
      include Chef::Mixin::ShellOut
      include Chef::Mixin::WindowsArchitectureHelper

      # Run a command under Blu with the same API as shell_out. The
      # options hash is extended to take an "architecture" flag which
      # can be set to :i386 or :x86_64 to force the windows architecture.
      # You can also add "credentials" to the options hash to set the
      # credentials as supported by Blu
      #
      # @param script [String] script to run
      # @param options [Hash] options hash
      # @return [Mixlib::Shellout] mixlib-shellout object
      def blu_script_out(*command_args)
        script = command_args.first
        options = command_args.last.is_a?(Hash) ? command_args.last : nil

        run_command_with_os_architecture(script, options)
      end

      # Run a command under blu with the same API as shell_out!
      # (raises exceptions on errors)
      #
      # @param script [String] script to run
      # @param options [Hash] options hash
      # @return [Mixlib::Shellout] mixlib-shellout object
      def blu_script_out!(*command_args)
        cmd = blu_script_out(*command_args)
        cmd.error!
        cmd
      end

      private

      # Helper function to run shell_out and wrap it with the correct
      # flags to possibly disable WOW64 redirection (which we often need
      # because chef-client runs as a 32-bit app on 64-bit windows).
      #
      # @param script [String] script to run
      # @param options [Hash] options hash
      # @return [Mixlib::Shellout] mixlib-shellout object
      def run_command_with_os_architecture(script, options)
        options ||= {}
        options = options.dup
        arch = options.delete(:architecture)
        runspace = options.delete(:runspace)

        with_os_architecture(nil, architecture: arch) do
          shell_out(
              build_blu_command(script, runspace),
              options
          )
        end
      end

      def build_blu_command(script, runspace)
        cmd = "blushell.exe exec --script-block \"#{script.gsub('"', '\"')}\""
        unless runspace.nil?
          cmd << " --runspace \"#{credentials}\""
        end
        cmd
      end
    end
  end
end
